using UnityEditor;
using UnityEngine;

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
                allEffects.effects.Add(allEffects.EffectToAdd.name, allEffects.EffectToAdd);
            }
            if (Application.isPlaying)
            {
                foreach (var p in allEffects.effects)
                {
                    EditorGUILayout.LabelField(p.Key + ": " + p.Value);
                }
            }
        }
    }

}