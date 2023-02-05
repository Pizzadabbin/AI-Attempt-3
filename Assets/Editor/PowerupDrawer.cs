using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Powerup))]
public class PowerupDrawer : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();

        Powerup powerup = (Powerup)target;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("IsRandom"));
        if(powerup.IsRandom) {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Strength"));
        } else {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Type"));
            if(powerup.Type == PowerupType.Health) {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Strength"));
            } else {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Duration"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("Strength"));
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
