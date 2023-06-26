using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovesData)), CanEditMultipleObjects]
public class MovesDataEditor : Editor
{
    public SerializedProperty
        moveName_Prop,
        description_Prop,
        moveType_Prop,
        elementalType_Prop,
        power_Prop,
        accuracy_Prop,
        statusEffect_Prop,
        statusAccuracy_Prop,
        unitStats_Prop;

    private void OnEnable()
    {
        moveName_Prop = serializedObject.FindProperty("moveName");
        description_Prop = serializedObject.FindProperty("description");
        moveType_Prop = serializedObject.FindProperty("moveType");
        elementalType_Prop = serializedObject.FindProperty("elementalType");
        power_Prop = serializedObject.FindProperty("power");
        accuracy_Prop = serializedObject.FindProperty("accuracy");
        statusEffect_Prop = serializedObject.FindProperty("statusEffect");
        statusAccuracy_Prop = serializedObject.FindProperty("statusAccuracy");
        unitStats_Prop = serializedObject.FindProperty("unitStats");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(moveName_Prop, new GUIContent("moveName"));
        EditorGUILayout.PropertyField(description_Prop, new GUIContent("description"));
        EditorGUILayout.PropertyField(moveType_Prop);
        MoveType mt = (MoveType)moveType_Prop.enumValueIndex;

        switch (mt)
        {
            case MoveType.PHYSICAL:
                EditorGUILayout.PropertyField(elementalType_Prop, new GUIContent("elementalType"));
                EditorGUILayout.IntSlider(power_Prop,0 , 160, new GUIContent("power"));
                EditorGUILayout.IntSlider(accuracy_Prop,0 ,100 , new GUIContent("accuracy"));
                break;

            case MoveType.SPECIAL:
                EditorGUILayout.PropertyField(elementalType_Prop, new GUIContent("elementalType"));
                EditorGUILayout.IntSlider(power_Prop, 0, 160, new GUIContent("power"));
                EditorGUILayout.IntSlider(accuracy_Prop, 0, 100, new GUIContent("accuracy"));
                break;

            case MoveType.BUFF:
                EditorGUILayout.PropertyField(elementalType_Prop, new GUIContent("elementalType"));
                EditorGUILayout.PropertyField(unitStats_Prop, new GUIContent("unitStats"));
                EditorGUILayout.IntSlider(statusAccuracy_Prop, 0, 100, new GUIContent("statusAccuracy"));
                break;

            case MoveType.DEBUFF:
                EditorGUILayout.PropertyField(elementalType_Prop, new GUIContent("elementalType"));
                EditorGUILayout.PropertyField(unitStats_Prop, new GUIContent("unitStats"));
                EditorGUILayout.IntSlider(statusAccuracy_Prop, 0, 100, new GUIContent("statusAccuracy"));
                break;

            case MoveType.STATUS:
                EditorGUILayout.PropertyField(elementalType_Prop, new GUIContent("elementalType"));
                EditorGUILayout.PropertyField(statusEffect_Prop, new GUIContent("unitStats"));
                EditorGUILayout.IntSlider(statusAccuracy_Prop, 0, 100, new GUIContent("statusAccuracy"));
                break;

        }

        serializedObject.ApplyModifiedProperties();
    }
}
