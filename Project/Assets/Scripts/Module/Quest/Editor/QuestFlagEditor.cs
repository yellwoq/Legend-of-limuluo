using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestFlag))]
public class QuestFlagEditor : Editor
{
    QuestFlag questFlag;
    SerializedProperty iconSpriteRenderer;
    SerializedProperty notAcceptedIcon;
    SerializedProperty acceptedIcon;
    SerializedProperty completeIcon;
    private void OnEnable()
    {
        questFlag=target as QuestFlag;
        iconSpriteRenderer = serializedObject.FindProperty("iconRenderer");
        notAcceptedIcon = serializedObject.FindProperty("notAccepted");
        acceptedIcon = serializedObject.FindProperty("accepted");
        completeIcon = serializedObject.FindProperty("complete");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUI.BeginChangeCheck();
        notAcceptedIcon.objectReferenceValue = EditorGUILayout.ObjectField("任务未接取时图标", notAcceptedIcon.objectReferenceValue as Sprite, typeof(Sprite), false);
        acceptedIcon.objectReferenceValue = EditorGUILayout.ObjectField("任务进行时图标", acceptedIcon.objectReferenceValue as Sprite, typeof(Sprite), false);
        completeIcon.objectReferenceValue = EditorGUILayout.ObjectField("任务完成时图标", completeIcon.objectReferenceValue as Sprite, typeof(Sprite), false);
        iconSpriteRenderer.objectReferenceValue = EditorGUILayout.ObjectField("图集渲染组件", questFlag.GetComponent<SpriteRenderer>(), typeof(SpriteRenderer), true);
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
    }
}
