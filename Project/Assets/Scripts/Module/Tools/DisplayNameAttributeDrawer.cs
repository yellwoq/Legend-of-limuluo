using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class DisplayNameAttribute : PropertyAttribute
{
    public string Name;
    public bool ReadOnly;
    public bool isEnumValue;
    public string[] memberNames;

    public DisplayNameAttribute(string name, bool readOnly = false, bool isEnumValue = false, params string[] memberNames)
    {
        Name = name;
        ReadOnly = readOnly;
        this.isEnumValue = isEnumValue;
        this.memberNames = memberNames;
    }
}