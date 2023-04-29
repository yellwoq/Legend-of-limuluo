using UnityEngine;
using UnityEditor;
using System;

[AttributeUsage(AttributeTargets.Field, Inherited = true)]
public class ReadOnlyAttribute : PropertyAttribute
{

}
