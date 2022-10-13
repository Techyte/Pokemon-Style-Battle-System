using UnityEditor;
using UnityEngine;
using PokemonGame.ScriptableObjects;

namespace PokemonGame
{
    [CustomEditor(typeof(AllStatusEffects))]
    public class AllStatusEffectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            AllStatusEffects allEffects = (AllStatusEffects)target;

            if (GUILayout.Button("Add Status Effect"))
            {
                if(!AllStatusEffects.effects.TryGetValue(allEffects.effectToAdd.name, out StatusEffect effect))
                {
                    AllStatusEffects.effects.Add(allEffects.effectToAdd.name, allEffects.effectToAdd);
                }
                else
                {
                    Debug.LogWarning("Item is already in the list, please do not try and add it again");
                }
                allEffects.effectToAdd = null;
            }
            if (Application.isPlaying)
            {
                foreach (var p in AllStatusEffects.effects)
                {
                    EditorGUILayout.LabelField(p.Key + ": " + p.Value);
                }
            }
        }
    }

}