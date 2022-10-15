﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace PokemonGame.ScriptableObjects
{
    /// <summary>
    /// A collection of every item in the game
    /// </summary>
    [CreateAssetMenu(fileName = "New All Items", menuName = "All/New All Items")]
    public class AllItems : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<string> keys = new List<string>();
        public List<Item> values = new List<Item>();

        /// <summary>
        /// The list of every item
        /// </summary>
        public static Dictionary<string, Item> items = new Dictionary<string, Item>();

        public Item itemToAdd;

        /// <summary>
        /// Attempts to get an item from the register and handles errors
        /// </summary>
        /// <param name="ItemName">The name of the item that you want to fetch</param>
        /// <param name="item">tThe outputted item</param>
        /// <returns></returns>
        public static bool GetItem(string ItemName, out Item item)
        {
            item = null;
            if (items.TryGetValue(ItemName, out Item itemToReturn))
            {
                item = itemToReturn;
                return true;
            }
            
            Debug.LogWarning("Item was not present in the register");
            return false;
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (var kvp in items)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            items = new Dictionary<string, Item>();

            for (int i = 0; i != Math.Min(keys.Count, values.Count); i++)
                items.Add(keys[i], values[i]);
        }
    }
}