using PokemonGame.Battle;
using UnityEngine;

namespace PokemonGame.Game
{
    public class BattleStarter : MonoBehaviour
    {
        public BattlerTemplate[] playerPartyTemplate;
        public BattlerTemplate[] npcStarterPokemon;
        public AllStatusEffects allStatusEffects;
        public Party playerParty;
        public Party apponentParty;

        public Move Ember;
        public Move Tackle;
        public Move Toxic;
        public Move RazorLeaf;

        public EnemyAI ai;

        private void Start()
        {
            LoadBattle();
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.white, 100);
        }

        private void Update()
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                LoadBattle();
            }
        }

        public void LoadBattle()
        {
            playerParty.party[0] = new Battler(
                playerPartyTemplate[0],
                5,
                allStatusEffects.effects["Healthy"],
                playerPartyTemplate[0].name,
                Ember,
                Tackle,
                Toxic,
                null);

            playerParty.party[1] = new Battler(
                playerPartyTemplate[1],
                5,
                allStatusEffects.effects["Healthy"],
                playerPartyTemplate[1].name,
                Tackle,
                Toxic,
                null,
                null);

            apponentParty.party[0] = new Battler(
                npcStarterPokemon[0],
                5,
                allStatusEffects.effects["Healthy"],
                npcStarterPokemon[0].name,
                Tackle,
                RazorLeaf,
                Toxic,
                null);

            BattleManager.LoadBattleScene(playerParty, apponentParty, ai);
        }
    }
}
