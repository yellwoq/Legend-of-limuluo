using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;



public class ScriptObjectIconEditor : Editor
{

    /// <summary>
    /// 获取类型
    /// </summary>
    /// <param name="TypeName"></param>
    /// <returns></returns>
    public static Type GetType(string TypeName)
    {
        var type = Type.GetType(TypeName);
        if (type != null)
            return type;
        //获取程序集名
        if (TypeName.Contains("."))
        {
            var assemblyName = TypeName.Substring(0, TypeName.IndexOf('.'));
            var assembly = Assembly.Load(assemblyName);
            if (assembly == null)
                return null;
            type = assembly.GetType(TypeName);
            if (type != null)
                return type;
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(TypeName);
                if (type != null)
                    return type;
            }
        }
        return null;
    }
}
