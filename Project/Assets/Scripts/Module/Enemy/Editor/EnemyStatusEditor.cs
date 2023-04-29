using UnityEditor;
using UnityEngine;

namespace Enemy
{
    [CustomEditor(typeof(EnemyStatus))]
    public class EnemyStatusEditor : Editor
    {
        EnemyStatus enemyStatus;
        SerializedProperty enemyInfo;
        SerializedProperty enemyID;
        SerializedProperty enemyName;
        SerializedProperty description;
        SerializedProperty horizontalValue;
        SerializedProperty vertcalValue;
        SerializedProperty sightDistance;
        SerializedProperty chParams;
        SerializedProperty DEM;
        SerializedProperty DEF;
        SerializedProperty currentHP;
        SerializedProperty maxHP;
        SerializedProperty moveSpeed;
        SerializedProperty Lv;

        private void OnEnable()
        {
            enemyStatus = target as EnemyStatus;
            enemyInfo = serializedObject.FindProperty("enemyInfo");
            enemyID = serializedObject.FindProperty("enemyID");
            enemyName = serializedObject.FindProperty("enemyName");
            description = serializedObject.FindProperty("description");
            horizontalValue = serializedObject.FindProperty("horizontalValue");
            vertcalValue = serializedObject.FindProperty("vertcalValue");
            sightDistance = serializedObject.FindProperty("sightDistance");
            chParams = serializedObject.FindProperty("chParams");
            DEM = serializedObject.FindProperty("DEM");
            DEF = serializedObject.FindProperty("DEF");
            currentHP = serializedObject.FindProperty("currentHP");
            maxHP = serializedObject.FindProperty("maxHP");
            moveSpeed = serializedObject.FindProperty("moveSpeed");
            Lv = serializedObject.FindProperty("Lv");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            enemyInfo.objectReferenceValue = EditorGUILayout.ObjectField("敌人信息", enemyInfo.objectReferenceValue as EnemyInformation, typeof(EnemyInformation), false);
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
            
            if (enemyInfo.objectReferenceValue)
            {
                EnemyInformation enemy = enemyStatus.EenemyInfo;
                enemyID.intValue = enemy.EnemyID;
                enemyName.stringValue = enemy.EnemyName;
                Lv.intValue = enemy.EnemyLv;
                DEM.floatValue = enemy.AttackPower;
                DEF.floatValue = enemy.DefencePower;
                maxHP.floatValue = enemy.MaxHP;
                description.stringValue = enemy.EnemyDes;
                GUI.enabled = false;
                if (enemy.EnemyIcon)
                    EditorGUILayout.ObjectField(new GUIContent("敌人图像"), enemy.EnemyIcon, typeof(Sprite), false);
                EditorGUILayout.PropertyField(enemyID, new GUIContent("敌人ID"));
                EditorGUILayout.PropertyField(enemyName, new GUIContent("敌人姓名"));
                EditorGUILayout.PropertyField(Lv, new GUIContent("敌人等级"));
                EditorGUILayout.PropertyField(DEM, new GUIContent("敌人攻击力"));
                EditorGUILayout.PropertyField(DEF, new GUIContent("敌人防御力"));
                EditorGUILayout.PropertyField(currentHP, new GUIContent("敌人当前血量"));
                EditorGUILayout.PropertyField(maxHP, new GUIContent("敌人最大血量"));
                GUI.enabled = false;
                EditorGUILayout.LabelField(new GUIContent("敌人描述信息"));
                EditorGUILayout.TextArea(description.stringValue);
                GUI.enabled = true;
            }
            GUI.enabled = false;
            EditorGUILayout.PropertyField(horizontalValue, new GUIContent("敌人当前水平轴值"));
            EditorGUILayout.PropertyField(vertcalValue, new GUIContent("敌人当前垂直轴值"));
            GUI.enabled = true;
            EditorGUILayout.PropertyField(chParams, new GUIContent("敌人动画参数"));
            EditorGUILayout.PropertyField(sightDistance, new GUIContent("敌人视野范围"));
            EditorGUILayout.PropertyField(moveSpeed, new GUIContent("敌人移动速度"));
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }
    }
}
