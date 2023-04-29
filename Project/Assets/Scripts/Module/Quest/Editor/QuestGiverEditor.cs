using DialogueSystem;
using MapSystem;
using UnityEditor;
using UnityEngine;

namespace QuestSystem
{
    [CustomEditor(typeof(QuestGiver))]
    public class QuestGiverEditor : Editor
    {
        QuestGiver questGiver;
        SerializedProperty currentTalkerInfo;
        SerializedProperty _ID;
        SerializedProperty _name;
        SerializedProperty heroIcon;
        SerializedProperty defaultDialogue;
        SerializedProperty characterType;
        SerializedProperty isVendor;
        SerializedProperty iconHolder;
        SerializedProperty currentPosition;

        private void OnEnable()
        {
            questGiver = target as QuestGiver;
            currentTalkerInfo = serializedObject.FindProperty("currentTalkerInfo");
            _ID = serializedObject.FindProperty("_ID");
            _name = serializedObject.FindProperty("_name");
            heroIcon = serializedObject.FindProperty("heroIcon");
            currentTalkerInfo = serializedObject.FindProperty("currentTalkerInfo");
            defaultDialogue = serializedObject.FindProperty("defaultDialogue");
            characterType = serializedObject.FindProperty("characterType");
            isVendor = serializedObject.FindProperty("isVendor");
            iconHolder = serializedObject.FindProperty("iconHolder");
            currentPosition = serializedObject.FindProperty("currentPosition");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space();
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            currentTalkerInfo.objectReferenceValue = EditorGUILayout.ObjectField("代表的人物", currentTalkerInfo.objectReferenceValue as TalkerInformation, typeof(TalkerInformation),true);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            if (currentTalkerInfo.objectReferenceValue)
            {

                TalkerInformation talkerInfo = currentTalkerInfo.objectReferenceValue as TalkerInformation;
                GUI.enabled = false;
                _ID.stringValue = talkerInfo.ID;
                _name.stringValue = talkerInfo.Name;
                characterType.enumValueIndex = (int)talkerInfo.ChType;
                isVendor.boolValue = talkerInfo.IsVendor;
                EditorGUILayout.PropertyField(_ID, new GUIContent("人物ID"));
                EditorGUILayout.PropertyField(_name, new GUIContent("人物姓名"));
                heroIcon.objectReferenceValue = EditorGUILayout.ObjectField("人物头像", talkerInfo.HeadIcon, typeof(Sprite), false);
                defaultDialogue.objectReferenceValue=EditorGUILayout.ObjectField("默认对话", talkerInfo.DefaultDialogue, typeof(Dialogue), false);
                EditorGUILayout.PropertyField(characterType, new GUIContent("人物种类"));
                EditorGUILayout.PropertyField(isVendor, new GUIContent("是否为商人"));
                GUI.enabled = true;
            }
            iconHolder.objectReferenceValue= EditorGUILayout.ObjectField("地图图标",iconHolder.objectReferenceValue as MapIconHolder, typeof(MapIconHolder),true);
            GUI.enabled = false;
            currentPosition.vector3Value= EditorGUILayout.Vector3Field("当前位置", questGiver.transform.position);
            GUI.enabled = true;
        }
    }
}
