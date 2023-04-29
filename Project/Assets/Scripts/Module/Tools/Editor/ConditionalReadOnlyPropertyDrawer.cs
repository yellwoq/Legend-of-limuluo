using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalReadOnlyAttribute))]
public class ConditionalReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalReadOnlyAttribute condRAtt = (ConditionalReadOnlyAttribute)attribute;
        bool enabled = GetConditionalReadOnlyAttributeResult(condRAtt, property);
        GUI.enabled = enabled;
        GUIContent content = null;
        if (condRAtt.Name != null)
            content = new GUIContent(label) { text = condRAtt.Name };
        else
            content = label;
        EditorGUI.PropertyField(position, property, content, true);
        GUI.enabled = true;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        ConditionalReadOnlyAttribute condRAtt = (ConditionalReadOnlyAttribute)attribute;
        return EditorGUI.GetPropertyHeight(property, new GUIContent(label) { text = condRAtt.Name }, true);
    }
    /// <summary>
    /// 获取条件只读结果
    /// </summary>
    /// <param name="condRAtt"></param>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool GetConditionalReadOnlyAttributeResult(ConditionalReadOnlyAttribute condRAtt, SerializedProperty property)
    {
        bool enabled = true;
        string propertyPath = property.propertyPath;
        //设置的布尔字段路径
        string conditionPath = propertyPath.Replace(property.propertyPath, condRAtt.ConditionalSourceField);
        SerializedProperty sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
        if (sourcePropertyValue != null)
        {
            //如果是枚举
            if (sourcePropertyValue.propertyType == SerializedPropertyType.Enum)
            {
                int enumValue = (int)Mathf.Pow(2, sourcePropertyValue.enumValueIndex);
                enabled = (enumValue & condRAtt.EnumCondition) == enumValue;

            }
            //如果是集合类型
            else if (sourcePropertyValue.propertyType == SerializedPropertyType.Generic)
            {
                // enabled = condRAtt.Negate ? !sourcePropertyValue : sourcePropertyValue.boolValue;

            }
            else enabled = condRAtt.Negate ? !sourcePropertyValue.boolValue : sourcePropertyValue.boolValue;
        }
        else
        {
            Debug.LogWarning("正在尝试使用ConditionalHideAttribute，但在objec中找不到匹配的SourcePropertyValue: " + condRAtt.ConditionalSourceField);
        }

        return enabled;
    }
}
