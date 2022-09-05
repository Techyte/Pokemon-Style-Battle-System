using System;
using System.Collections;
using System.Collections.Generic;
using PokemonGame.Dialogue;
using PokemonGame.ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace PokemonGame.Game
{
    /// <summary>
    /// Initiates a battle based on certain inspector parameters 
    /// </summary>
    public class BattleStarter : DialogueTrigger
    {
        public AllStatusEffects allStatusEffects;
        public AllMoves allMoves;
        public Party playerParty;
        public Party opponentParty;

        public NavMeshAgent agent;

        public EnemyAI ai;

        public bool isDefeated;
        private bool _hasTalkedDefeatedText;

        private bool hasFinishedStartText;

        [SerializeField] private Transform playerSpawnPos;
        
        public int battlerId;

        [SerializeField] private TextAsset StartBattleText;
        [SerializeField] private TextAsset DefeatedBattleText;

        private GameLoader _gameLoader;

        private void OnValidate()
        {
            Register();
            if (!_gameLoader)
                _gameLoader = FindObjectOfType<GameLoader>();
        }

        private void Awake()
        {
            Register();
        }

        private void Register()
        {
            if (battlerId == 0)
            {
                int newBattlerId = BattleStarterRegister.battleStarters.Count+1;
                BattleStarterRegister.battleStarters.Add(this);
                battlerId = newBattlerId;   
            }
        }

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();

            DialogueFinished += StartingDialogueEnded;
        }

        /// <summary>
        /// Triggers the defeated dialogue
        /// </summary>
        public void Defeated()
        {
            isDefeated = true;
            DialogueFinished -= StartingDialogueEnded;
            StartCoroutine(StartDefeatedDialogue());
        }

        private IEnumerator StartDefeatedDialogue()
        {
            yield return new WaitForEndOfFrame();
            StartDialogue(DefeatedBattleText);
            _gameLoader.player.LookAtTrainer(transform.position);
        }

        private bool hasStartedWalking;
        private bool hasStartedTalkingStartText;
        private void Update()
        {
            //Detecting when the player goes in front of the battle starter
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
            {
                if (!isDefeated)
                {
                    Player player = hit.transform.gameObject.GetComponent<Player>();
                    if (player)
                    {
                        if(hit.transform.position.x == transform.position.x)
                        {
                            PlayerMovement playerMovement = hit.transform.gameObject.GetComponent<PlayerMovement>();

                            playerMovement.battleStarterHasStartedWalking = true;
                            agent.destination = hit.transform.position;

                            player.LookAtTrainer(transform.position);

                            if (agent.velocity.magnitude > 0.15f)
                            {
                                hasStartedWalking = true;
                            }

                            if (hasStartedWalking)
                            {
                                if (agent.velocity.magnitude < 0.15f && !hasStartedTalkingStartText)
                                {
                                    StartDialogue(StartBattleText);
                                    hasStartedTalkingStartText = true;
                                }   
                            }

                            if (hasFinishedStartText)
                            {
                                LoadBattle();
                            }   
                        }
                    }
                }
            }
        }

        private void StartingDialogueEnded(object sender, EventArgs args)
        {
            hasFinishedStartText = true;
        }

        private void LoadBattle()
        {
            for (int i = 0; i < playerParty.party.Count; i++)
            {
                if (playerParty.party[i])
                {
                    Battler replacementBattler = Battler.CreateCopy(playerParty.party[i]);
                    playerParty.party[i] = replacementBattler;   
                }
            }
            
            for (int i = 0; i < opponentParty.party.Count; i++)
            {
                if (opponentParty.party[i])
                {
                    Battler replacementBattler = Battler.CreateCopy(opponentParty.party[i]);
                    opponentParty.party[i] = replacementBattler;   
                }
            }
            
            object[] vars = { playerParty, opponentParty, ai, playerSpawnPos.position, battlerId};
            SceneLoader.LoadScene(1, vars);
        }
    }

    public class BattleStarterRegister
    {
        public static List<BattleStarter> battleStarters = new List<BattleStarter>();
    }
}
