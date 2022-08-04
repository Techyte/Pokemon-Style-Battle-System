using System;
using System.Collections.Generic;
using Ink.Parsed;
using PokemonGame.Battle;
using UnityEngine;
using UnityEngine.AI;

namespace PokemonGame.Game
{
    /// <summary>
    /// Initiates a battle based on certain inspector parameters 
    /// </summary>
    public class BattleStarter : MonoBehaviour
    {
        public AllStatusEffects allStatusEffects;
        public AllMoves allMoves;
        public Party playerParty;
        public Party opponentParty;

        public NavMeshAgent agent;

        public EnemyAI ai;

        private bool _hasStartedWalking;
        public bool isDefeated;
        private bool _hasTalkedDefeatedText;

        [SerializeField] private Transform playerSpawnPos;
        
        public int battlerId;

        private void OnValidate()
        {
            Register();
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
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Triggers the defeated dialogue
        /// </summary>
        public void Defeated()
        {
            _hasTalkedDefeatedText = true;
        }

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
                        PlayerMovement playerMovement = hit.transform.gameObject.GetComponent<PlayerMovement>();

                        playerMovement.battleStarterHasStartedWalking = true;
                        agent.destination = hit.transform.position;

                        player.LookAtTrainer(transform.position);

                        Invoke(nameof(HasStartedWalkingSetter), .1f);
                    
                        if (agent.velocity.magnitude < 0.15f && _hasStartedWalking)
                        {
                            LoadBattle();
                        }   
                    }
                }
                else
                {
                    if(isDefeated && !_hasTalkedDefeatedText)
                        Defeated();
                }
            }
        }

        private void HasStartedWalkingSetter()
        {
            _hasStartedWalking = true;
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
