using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |AttributeTargets.Class | AttributeTargets.Struct|AttributeTargets.GenericParameter, Inherited = true)]
public class ConditionalReadOnlyAttribute : PropertyAttribute
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
    public bool ReadOnly = false;
    /// <summary>
    /// 属性名字
    /// </summary>
    public string Name;
    /// <summary>
    /// 当条件是指定的值时才显示
    /// </summary>
    /// <param name="conditionalSourceField"></param>
    /// <param name="readOnly"></param>
    /// <param name="negate"></param>
    public ConditionalReadOnlyAttribute(string conditionalSourceField, bool readOnly = false, bool negate = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ReadOnly = readOnly;
        this.Negate = negate;
    }
    public ConditionalReadOnlyAttribute(string name, string conditionalSourceField, bool readOnly = false, bool negate = false)
    {
        this.Name = name;
        this.ConditionalSourceField = conditionalSourceField;
        this.ReadOnly = readOnly;
        this.Negate = negate;
    }
    /// <summary>
    /// 指定必须是枚举值才显示在面板中
    /// </summary>
    /// <param name="conditionalSourceField"></param>
    /// <param name="enumCondition"></param>
    /// <param name="hideInInspector"></param>
    public ConditionalReadOnlyAttribute(string conditionalSourceField, int enumCondition, bool readOnly = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ReadOnly = readOnly;
        this.EnumCondition = enumCondition;
    }
    ///<summary>
    /// 指定必须是枚举值才显示在面板中
    /// </summary>
    /// <param name="conditionalSourceField"></param>
    /// <param name="enumCondition"></param>
    /// <param name="hideInInspector"></param>
    public ConditionalReadOnlyAttribute(string name, string conditionalSourceField, int enumCondition, bool readOnly = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.ReadOnly = readOnly;
        this.EnumCondition = enumCondition;
    }
}
