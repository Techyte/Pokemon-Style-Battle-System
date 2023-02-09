namespace PokemonGame.Battle
{
    using System.Collections.Generic;
    using Game.Party;
    using General;
    using Global;
    using ScriptableObjects;
    using UnityEngine;
    using Random = UnityEngine.Random;

    public enum TurnStatus
    {
        Choosing,
        Showing,
        Ending
    }

    /// <summary>
    /// The main class that manages battles
    /// </summary>
    public class Battle : MonoBehaviour
    {
        private static Battle _singleton;
        public static Battle Singleton
        {
            get => _singleton;
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else if (_singleton != value)
                {
                    Debug.Log($"{nameof(Battle)} instance already exists, destroying duplicate!");
                    Destroy(value);
                }
            }
        }

        private void Awake()
        {
            Singleton = this;
        }

        [Header("UI:")]
        [SerializeField] private BattleUIManager uiManager;

        [Space]
        [Header("Assignments")]
        public int currentBattlerIndex;

        public int opponentBattlerIndex;

        [Space]
        [Header("Other Readouts")]
        [SerializeField] public TurnStatus currentTurn = TurnStatus.Choosing;
        
        public Party playerParty;
        
        public Party opponentParty;
        
        [SerializeField] private EnemyAI enemyAI;
        
        [SerializeField] private Move playerMoveToDo;
        
        public Move enemyMoveToDo;
        
        [SerializeField] private bool playerHasChosenAttack;
        
        [SerializeField] private bool hasDoneChoosingUpdate;
        
        [SerializeField] private bool hasShowedMoves;

        private Battler playerCurrentBattler => playerParty[currentBattlerIndex];

        private Battler opponentCurrentBattler => opponentParty[opponentBattlerIndex];

        
        private string _opponentName;

        private Vector3 _playerPos;
        private Quaternion _playerRotation;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //Loads relevant info like the opponent and player party
            Debug.Log("First");
            playerParty = PartyManager.Instance.GetParty();
            opponentParty = (Party)SceneLoader.GetVariable("opponentParty");
            enemyAI = (EnemyAI)SceneLoader.GetVariable("enemyAI");
            _opponentName = (string)SceneLoader.GetVariable("opponentName");
            _playerPos = (Vector3)SceneLoader.GetVariable("playerPosition");
            _playerRotation = (Quaternion)SceneLoader.GetVariable("playerRotation");

            SceneLoader.ClearLoader();

            currentBattlerIndex = 0;
            opponentBattlerIndex = 0;
        }

        private void Update()
        {
            if (playerHasChosenAttack)
            {
                currentTurn = TurnStatus.Showing;
            }

            switch (currentTurn)
            {
                case TurnStatus.Ending:
                    TurnEnding();
                    break;
                case TurnStatus.Showing:
                    TurnShowing();
                    break;
                case TurnStatus.Choosing:
                    if (!hasDoneChoosingUpdate)
                    {
                        uiManager.ShowUI(true);
                        enemyAI.AIMethod(new AIMethodEventArgs(opponentCurrentBattler, opponentParty));
                        hasDoneChoosingUpdate = true;
                    }
                    break;
            }
        }

        private void TurnShowing()
        {
            if (!hasShowedMoves)
            {
                hasShowedMoves = true;
                uiManager.ShowUI(false);
                DoMoves();
                playerHasChosenAttack = false;
                currentTurn = TurnStatus.Ending;
            }
        }

        private void TurnEnding()
        {
            uiManager.ShowUI(false);
            hasDoneChoosingUpdate = false;
            hasShowedMoves = false;
            playerHasChosenAttack = false;
            DoStatusEffects();
            currentTurn = TurnStatus.Choosing;
        }

        private void DoStatusEffects()
        {
            opponentCurrentBattler.statusEffect.Effect(new StatusEffectEventArgs(
                opponentCurrentBattler));

            playerCurrentBattler.statusEffect.Effect(new StatusEffectEventArgs(
                playerCurrentBattler));
            
            uiManager.UpdateHealthDisplays();
            CheckForWinCondition();
        }

        //Public method used by the move UI buttons
        public void ChooseMove(int moveID)
        {
            playerMoveToDo = playerCurrentBattler.moves[moveID];
            playerHasChosenAttack = true;
        }

        private void DoPlayerMove()
        {
            //You can add any animation calls for attacking here

            if (playerMoveToDo)
            {
                if (playerMoveToDo.category == MoveCategory.Status)
                {
                    playerMoveToDo.MoveMethod(new MoveMethodEventArgs(opponentCurrentBattler));
                }
                else
                {
                    int damageToDo = CalculateDamage(playerMoveToDo, playerCurrentBattler, opponentCurrentBattler);
                    opponentCurrentBattler.TakeDamage(Mathf.RoundToInt(damageToDo));
                }
            }

            uiManager.UpdateHealthDisplays();
            CheckForWinCondition();
        }

        private int CalculateDamage(Move move, Battler battlerThatUsed, Battler battlerBeingAttacked)
        {
            //Checking to see if the move is capable of hitting the opponent battler
            foreach (var hType in move.type.cantHit)
            {
                if (hType == battlerBeingAttacked.primaryType || hType == battlerBeingAttacked.secondaryType)
                {
                    Debug.Log(move.type + " can't hit that battler");
                    return 0;
                }
            }

            float type = 1;

            //Calculating type disadvantages
            foreach (var weakType in move.type.weakAgainst)
            {
                if (weakType == battlerBeingAttacked.primaryType)
                {
                    type /= 2;
                }
                if (weakType == battlerBeingAttacked.secondaryType)
                {
                    type /= 2;
                }
            }

            //Calculating type advantages
            foreach (var strongType in move.type.strongAgainst)
            {
                if (strongType == battlerBeingAttacked.primaryType)
                {
                    type *= 2;
                }
                if (strongType == battlerBeingAttacked.secondaryType)
                {
                    type *= 2;
                }
            }

            //Failsafe
            if (type > 4)
                type = 4;
            if (type < .25f)
                type = .25f;

            //STAB =  Same type attack bonus
            int stab = 1;
            if (move.type == battlerThatUsed.primaryType)
            {
                stab = 2;
            }

            //Damage calculation is correct (took me way to long to get it right) source: https://bulbapedia.bulbagarden.net/wiki/Damage#Generation_II
            int damage = move.category == MoveCategory.Physical
                ? Mathf.RoundToInt(((2 * battlerThatUsed.level / 5 + 2) * move.damage *
                    (battlerThatUsed.attack / battlerBeingAttacked.defense) / 50 + 2) * stab * type)
                : Mathf.RoundToInt(((2 * battlerThatUsed.level / 5 + 2) * move.damage *
                    (battlerThatUsed.specialAttack / battlerBeingAttacked.specialDefense) / 50 + 2) * stab * type);

            int randomness = Mathf.RoundToInt(Random.Range(.8f * damage, damage * 1.2f));
            damage = randomness;

            return damage;
        }

        private void DoMoves()
        {
            if(playerCurrentBattler.speed > opponentCurrentBattler.speed)
            {
                //Player is faster
                DoPlayerMove();
                DoEnemyMove();
            }
            else
            {
                //Enemy is faster
                DoEnemyMove();
                DoPlayerMove();
            }

            enemyMoveToDo = null;
            playerMoveToDo = null;
        }

        private void DoEnemyMove()
        {
            //You can add any animation calls for attacking here

            if (enemyMoveToDo.category == MoveCategory.Status)
            {
                enemyMoveToDo.MoveMethod(new MoveMethodEventArgs(playerCurrentBattler));
            }
            else
            {
                float damageToDo = CalculateDamage(enemyMoveToDo, opponentCurrentBattler, playerCurrentBattler);
                playerCurrentBattler.TakeDamage(Mathf.RoundToInt(damageToDo));
            }
            
            if (playerCurrentBattler.isFainted)
            {
                uiManager.SwitchBattlerBecauseOfDeath();
            }
            
            CheckForWinCondition();

            uiManager.UpdateHealthDisplays();
        }

        private void EndBattle(bool isDefeated)
        {
            Dictionary<string, object> vars = new Dictionary<string, object>
            {
                { "playerParty", playerParty },
                { "trainerName", _opponentName },
                { "playerPos", _playerPos },
                { "playerRotation", _playerRotation },
                { "isDefeated", isDefeated }
            };

            SceneLoader.LoadScene("Game", vars);
        }

        private void CheckForWinCondition()
        {
            //Counting how many fainted battlers in the players party
            var playerFaintedPokemon = 0;
            var playerPartyCount = 0;

            for (int i = 0; i < playerParty.Count; i++)
            {
                if (playerParty[i])
                {
                    playerPartyCount++;
                    if (playerParty[i].isFainted)
                        playerFaintedPokemon++;
                }
            }
            
            if (playerFaintedPokemon == playerPartyCount)
            {
                Debug.Log("Battle ended because player lost all battlers");
                EndBattle(true);
            }

            //Counting how many fainted battlers in the opponent party
            int enemyFaintedPokemon = 0;
            int enemyPartyCount = 0;

            for (int i = 0; i < opponentParty.Count; i++)
            {
                if (opponentParty[i])
                {
                    enemyPartyCount++;
                    if (opponentParty[i].isFainted)
                        enemyFaintedPokemon++;
                }
            }

            if (enemyFaintedPokemon == enemyPartyCount)
            {
                Debug.Log("Battle ended because enemy lost all battlers");
                EndBattle(true);
            }
        }
    }   
}