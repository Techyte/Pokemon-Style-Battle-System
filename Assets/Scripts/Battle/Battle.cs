using UnityEngine;

namespace PokemonGame.Battle
{
    public enum TurnStatus
    {
        Choosing,
        Showing,
        Ending
    }

    public class Battle : MonoBehaviour
    {
        [Header("UI:")]
        public GameObject playerUIHolder;
        [SerializeField] private BattleUIManager uiManager;

        [Space]
        [Header("Assignments")]
        public int currentBattlerIndex;

        public int opponentBattlerIndex;

        [Space]
        [Header("Other Readouts")]
        public TurnStatus currentTurn = TurnStatus.Choosing;
        public Party playerParty;
        public Party opponentParty;

        private EnemyAI _enemyAI;

        private Move _playerMoveToDo;
        public Move enemyMoveToDo;
        public bool playerHasChosenAttack;
        [SerializeField]private bool hasDoneChoosingUpdate;
        [SerializeField]private bool hasShowedMoves;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            playerParty = LoaderInfo.playerParty;
            opponentParty = LoaderInfo.opponentParty;
            _enemyAI = LoaderInfo.enemyAI;

            BattleManager.ClearLoader();

            currentBattlerIndex = 0;

            opponentBattlerIndex = 0;

            opponentParty.party[opponentBattlerIndex].currentHealth = opponentParty.party[opponentBattlerIndex].maxHealth;
            playerParty.party[currentBattlerIndex].currentHealth = playerParty.party[currentBattlerIndex].maxHealth;
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
                        _enemyAI.aiMethod(opponentParty.party[opponentBattlerIndex], opponentParty, this);
                        hasDoneChoosingUpdate = true;
                    }
                    break;
            }
        }

        private void TurnShowing()
        {
            if (!hasShowedMoves)
            {
                uiManager.ShowUI(false);
                DoMoves();
                hasShowedMoves = true;
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
            opponentParty.party[opponentBattlerIndex].statusEffect.effect(opponentParty.party[opponentBattlerIndex]);

            playerParty.party[currentBattlerIndex].statusEffect.effect(playerParty.party[currentBattlerIndex]);
            uiManager.UpdateHealthDisplays();
        }

        public void ChooseMove(int moveID)
        {
            _playerMoveToDo = playerParty.party[currentBattlerIndex].moves[moveID];
            playerHasChosenAttack = true;
        }

        private void DoPlayerMove()
        {
            //You can add any animation calls for attacking here

            if (_playerMoveToDo != null)
            {

                if (_playerMoveToDo.category == MoveCategory.Status)
                {
                    _playerMoveToDo.moveMethod(opponentParty.party[opponentBattlerIndex]);
                }
                else
                {
                    double damageToDo = CalculateDamage(_playerMoveToDo, playerParty.party[currentBattlerIndex], opponentParty.party[opponentBattlerIndex]);
                    opponentParty.party[opponentBattlerIndex].currentHealth -= (int)damageToDo;
                }

                if (opponentParty.party[opponentBattlerIndex].currentHealth <= 0)
                {
                    opponentParty.party[opponentBattlerIndex].isFainted = true;
                }
            }

            CheckForWinCondition();

            uiManager.UpdateHealthDisplays();
        }

        private double CalculateDamage(Move move, Battler battlerThatUsed, Battler battlerBeingAttacked)
        {
            foreach (var hType in move.type.cantHit)
            {
                if (hType == battlerBeingAttacked.primaryType || hType == battlerBeingAttacked.secondaryType)
                {
                    Debug.Log(move.type + " can't hit that battler");
                    return 0;
                }
            }

            double type = 1;

            foreach (var hType in move.type.strongAgainst)
            {
                if (hType == battlerBeingAttacked.primaryType || hType == battlerBeingAttacked.secondaryType)
                {
                    type = 1.5f;
                }
            }

            foreach (var hType in move.type.weakAgainst)
            {
                if (hType == battlerBeingAttacked.primaryType || hType == battlerBeingAttacked.secondaryType)
                {
                    //Debug.Log(move.type + " is weak against " + move.type.weakAgainst[i]);
                    if (type == 1.5)
                    {
                        type = 1;
                    }
                    else
                    {
                        type = .5f;
                    }
                }
            }

            int stab = 1;
            if (move.type == battlerThatUsed.primaryType)
            {
                stab = 2;
            }

            if (move.type == battlerThatUsed.secondaryType)
            {
                if (type == 2)
                {
                    type += 2;
                }
                else
                {
                    type = 2;
                }
            }

            //Damage calculation is correct
            double damage = move.category == MoveCategory.Physical
                ? ((2 * battlerThatUsed.level / 5 + 2) * move.damage *
                    (battlerThatUsed.attack / battlerBeingAttacked.defense) / 50 + 2) * stab * type
                : ((2 * battlerThatUsed.level / 5 + 2) * move.damage *
                    (battlerThatUsed.specialAttack / battlerBeingAttacked.specialDefense) / 50 + 2) * stab * type;

            float randomness = Mathf.RoundToInt(Random.Range(.8f * (float)damage, (float)damage * 1.2f));
            damage = randomness;

            return damage;
        }

        private void DoMoves()
        {
            if(opponentParty.party[opponentBattlerIndex].speed > playerParty.party[currentBattlerIndex].speed)
            {
                //Enemy is faster
                DoEnemyMove();
                DoPlayerMove();
            }
            else
            {
                //Player is faster
                DoPlayerMove();
                DoEnemyMove();
            }

            uiManager.UpdateHealthDisplays();
        }

        public void DoMoveOnPlayer(Move move)
        {
            enemyMoveToDo = move;
        }

        private void DoEnemyMove()
        {
            //You can add any animation calls for attacking here

            if (enemyMoveToDo.category == MoveCategory.Status)
            {
                enemyMoveToDo.moveMethod(opponentParty.party[currentBattlerIndex]);
            }
            else
            {
                double damageToDo = CalculateDamage(enemyMoveToDo, opponentParty.party[opponentBattlerIndex], playerParty.party[currentBattlerIndex]);
                playerParty.party[currentBattlerIndex].currentHealth -= (int)damageToDo;
            }

            if (playerParty.party[currentBattlerIndex].currentHealth <= 0)
            {
                playerParty.party[currentBattlerIndex].isFainted = true;
                uiManager.SwitchBattlerBecauseOfDeath();
            }

            CheckForWinCondition();

            uiManager.UpdateHealthDisplays();
        }

        private void EndBattle()
        {
            //Debug.Log("Ending Battle");

            var playerPath = Application.persistentDataPath + "/party.json";
            var opponentPath = Application.persistentDataPath + "/opponentTestParty.json";

            SaveAndLoad<Party>.SaveJson(playerParty, playerPath);
            SaveAndLoad<Party>.SaveJson(opponentParty, opponentPath);

            BattleManager.LoadScene(SaveAndLoad<Party>.LoadJson(playerPath), SaveAndLoad<Party>.LoadJson(opponentPath), null, 0);
        }

        private void CheckForWinCondition()
        {
            var playerFaintedPokemon = 0;
            var playerPartyCount = 0;

            foreach (var partyPokemon in playerParty.party)
            {
                if (partyPokemon)
                {
                    playerPartyCount++;
                }
            }

            foreach (var partyPokemon in playerParty.party)
            {
                if (partyPokemon)
                    if (partyPokemon.isFainted)
                        playerFaintedPokemon++;
            }

            if (playerFaintedPokemon == playerPartyCount)
            {
                GameWorldData.fromBattle = true;
                GameWorldData.isDefeated = false;
                EndBattle();
            }

            int enemyFaintedPokemon = 0;
            int enemyPartyCount = 0;

            foreach (var partyPokemon in opponentParty.party)
            {
                if (partyPokemon)
                {
                    enemyPartyCount++;
                }
            }

            foreach (var partyPokemon in opponentParty.party)
            {
                if(partyPokemon)
                    if (partyPokemon.isFainted)
                        enemyFaintedPokemon++;
            }

            if (enemyFaintedPokemon == enemyPartyCount)
            {
                GameWorldData.fromBattle = true;
                GameWorldData.isDefeated = true;
                EndBattle();
            }
        }
    }
}