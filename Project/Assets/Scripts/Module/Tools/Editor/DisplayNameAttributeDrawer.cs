using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DisplayNameAttribute))]
public class DisplayNameAttributeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DisplayNameAttribute displayAttribute = this.attribute as DisplayNameAttribute;
        if (displayAttribute.isEnumValue)
        {
            GUIContent[] contents = new GUIContent[displayAttribute.memberNames.Length];
            for (int i = 0; i < contents.Length; i++)
            {
                GUIContent content = new GUIContent(label) { text = displayAttribute.memberNames[i] };
                contents[i] = content;
            }
            if (!displayAttribute.ReadOnly)
            {
                property.enumValueIndex = EditorGUI.Popup(position, new GUIContent(label) { text = displayAttribute.Name }, property.enumValueIndex, contents);
            }
            else
            {
                GUI.enabled = false;
                property.enumValueIndex = EditorGUI.Popup(position, new GUIContent(label) { text = displayAttribute.Name }, property.enumValueIndex, contents);
                GUI.enabled = true;
            }
        }
        else
        {
            if (!displayAttribute.ReadOnly)
            {
                EditorGUI.PropertyField(position, property, new GUIContent(label) { text = displayAttribute.Name }, true);
            }
            else
            {
                GUI.enabled = false;
                EditorGUI.PropertyField(position, property, new GUIContent(label) { text = displayAttribute.Name }, true);
                GUI.enabled = true;
            }
        }
    }
}
