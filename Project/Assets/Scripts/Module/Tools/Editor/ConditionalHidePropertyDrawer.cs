using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);
        bool wasEnabled = GUI.enabled;
        GUI.enabled = enabled;
        if (!condHAtt.HideInInspector || enabled)
        {
            GUIContent content = null;
            if (condHAtt.Name != null)
                content = new GUIContent(label) { text = condHAtt.Name };
            else
                content = label;
            EditorGUI.PropertyField(position, property, content, true);
        }

        GUI.enabled = wasEnabled;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
        bool enabled = GetConditionalHideAttributeResult(condHAtt, property);

        if (!condHAtt.HideInInspector || enabled)
        {
            GUIContent content = null;
            if (condHAtt.Name != null)
                content = new GUIContent(label) { text = condHAtt.Name };
            else
                content = label;
            return EditorGUI.GetPropertyHeight(property, content, true);
        }
        else
        {
            return -EditorGUIUtility.standardVerticalSpacing;
        }
    }
    /// <summary>
    /// 获取条件隐藏结果
    /// </summary>
    /// <param name="condHAtt"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool GetConditionalHideAttributeResult(ConditionalHideAttribute condHAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        //设置的布尔字段路径
        string conditionPath = propertyPath.Replace(property.propertyPath, condHAtt.ConditionalSourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        if (sourcePropertyValue != null)
        {
            //如果是枚举属性
            if (sourcePropertyValue.propertyType == SerializedPropertyType.Enum)
            {
                int enumValue = (int)Mathf.Pow(2, sourcePropertyValue.enumValueIndex);
                enabled = (enumValue & condHAtt.EnumCondition) == enumValue;

            }
            else enabled = condHAtt.Negate ? !sourcePropertyValue.boolValue : sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("正在尝试使用ConditionalHideAttribute，但在object中找不到匹配的SourcePropertyValue: " + condHAtt.ConditionalSourceField);
        }

        return enabled;
    }
}