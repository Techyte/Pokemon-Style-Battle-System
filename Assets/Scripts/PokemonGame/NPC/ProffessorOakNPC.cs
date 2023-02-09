namespace PokemonGame.NPC
{
    using Game.Party;
    using General;
    using ScriptableObjects;
    using UnityEngine;

    public class ProffessorOakNPC : NPC
    {
        [SerializeField] private BattlerTemplate charmander;
        [SerializeField] private BattlerTemplate bulbasaur;
        [SerializeField] private BattlerTemplate squirtle;
        
        [SerializeField] private TextAsset textAsset;
        
        protected override void OnPlayerInteracted()
        {
            StartDialogue(textAsset);
            base.OnPlayerInteracted();
        }
        
        public override void CallTag(string tagKey, string tagValue)
        {
            switch (tagKey)
            {
                case "Charmander": 
                    PartyManager.Instance.AddBattler(Battler.Init(charmander, 5, null, "Charmander", null, null, null, null, true));
                    break;
                case "Squirtle":
                    PartyManager.Instance.AddBattler(Battler.Init(squirtle, 5, null, "Squirtle", null, null, null, null, true));
                    break;
                case "Bulbasaur":
                    PartyManager.Instance.AddBattler(Battler.Init(bulbasaur, 5, null, "Bulbasaur", null, null, null, null, true));
                    break;
            }
        }
    }
}