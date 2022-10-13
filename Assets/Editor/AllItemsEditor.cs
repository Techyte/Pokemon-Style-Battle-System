﻿using UnityEditor;
using UnityEngine;
using PokemonGame.ScriptableObjects;

namespace PokemonGame
{
    [CustomEditor(typeof(AllItems))]
    public class AllItemsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AllItems allItems = (AllItems)target;

            if (GUILayout.Button("Add Item"))
            {
                if(!AllItems.items.TryGetValue(allItems.itemToAdd.name, out Item item))
                {
                    AllItems.items.Add(allItems.itemToAdd.name, allItems.itemToAdd);
                }
                else
                {
                    Debug.LogWarning("Item is already in the list, please do not try and add it again");
                }
                allItems.itemToAdd = null;
            }
            if (Application.isPlaying)
            {
                foreach (var p in AllItems.items)
                {
                    EditorGUILayout.LabelField(p.Key + ": " + p.Value);
                }
            }
        }
    }
}