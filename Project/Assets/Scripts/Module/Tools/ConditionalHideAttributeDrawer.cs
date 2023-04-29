using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 条件隐藏属性
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalHideAttribute : PropertyAttribute
{
    /// <summary>
    /// 将要控制中的布尔字段的名称
    /// </summary>
    public string ConditionalSourceField = "";
    /// <summary>
    /// TRUE=将控制的布尔字段取反
    /// </summary>
    public bool Negate;
    /// <summary>
    /// 记录值如果字段是枚举类型，则枚举元素的值必须是2的幂
    /// </summary>
    public int EnumCondition;
    /// <summary>
    /// TRUE=在检查器中隐藏/FALSE=在检查器中禁用
    /// </summary>
    public bool HideInInspector = false;
    /// <summary>
    /// 属性名字
    /// </summary>
    public string Name;
    /// <summary>
    /// 当条件是指定的值时才显示
    /// </summary>
    /// <param name="conditionalSourceField"></param>
    /// <param name="hideInInspector"></param>
    /// <param name="negate"></param>
    public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector = false, bool negate = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.Negate = negate;
    }
    public ConditionalHideAttribute(string name, string conditionalSourceField, bool hideInInspector = false, bool negate = false)
    {
        this.Name = name;
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.Negate = negate;
    }
    /// <summary>
    /// 指定必须是枚举值才显示在面板中
    /// </summary>
    /// <param name="conditionalSourceField"></param>
    /// <param name="enumCondition"></param>
    /// <param name="hideInInspector"></param>
    public ConditionalHideAttribute(string conditionalSourceField, int enumCondition, bool hideInInspector = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.EnumCondition = enumCondition;
    }
    ///<summary>
    /// 指定必须是枚举值才显示在面板中
    /// </summary>
    /// <param name="conditionalSourceField"></param>
    /// <param name="enumCondition"></param>
    /// <param name="hideInInspector"></param>
    public ConditionalHideAttribute(string name, string conditionalSourceField, int enumCondition, bool hideInInspector = false)
    {
        this.Name = name;
        this.ConditionalSourceField = conditionalSourceField;
        this.HideInInspector = hideInInspector;
        this.EnumCondition = enumCondition;
    }
}

