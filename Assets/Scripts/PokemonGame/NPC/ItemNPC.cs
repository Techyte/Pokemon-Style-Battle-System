namespace PokemonGame.NPC
{
    using General;
    using ScriptableObjects;
    using UnityEngine;
    using Global;

    public class ItemNPC : NPC
    {
        [SerializeField] private TextAsset textAsset;

        protected override void OnPlayerInteracted()
        {
            StartDialogue(textAsset);
            base.OnPlayerInteracted();
        }
    }
}