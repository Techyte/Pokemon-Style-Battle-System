namespace PokemonGame.Battle
{
    using UnityEngine;
    using ScriptableObjects;

    /// <summary>
    /// Contains all the logic for every move
    /// </summary>
    [CreateAssetMenu(fileName = "New Item Methods", menuName = "All/New Item Methods")]
    public class ItemMethods : ScriptableObject
    {
        public void Potion(ItemMethodEventArgs e)
        {
            if (!e.target.isFainted)
            {
                e.target.Heal(20);
                e.success = true;
            }
        }
        
        public void Revive(ItemMethodEventArgs e)
        {
            if (e.target.isFainted)
            {
                e.target.Revive(false);
                e.success = true;
            }
        }
        
        public void MaxRevive(ItemMethodEventArgs e)
        {
            if (e.target.isFainted)
            {
                e.target.Revive(true);
                e.success = true;
            }
        }
    }   
}