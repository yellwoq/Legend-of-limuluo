using System;
using UnityEngine;
using UnityEngine.Events;

namespace Fungus
{
    [CommandInfo("Scripting",
               "MyInvoke Method",
               "Invokes a method of a component via reflection. Supports passing multiple parameters and storing returned values in a Fungus variable.")]
    public class MyInvokeMethod : InvokeMethod
    {
        [SerializeField]
        protected string mytargetComponentAssemblyName;
        [SerializeField]
        protected string mytargetMethod;
        protected override void Awake()
        {
            if (componentType == null)
            {
                componentType = Type.GetType(mytargetComponentAssemblyName);
            }
            if (objComponent == null)
            {
                Debug.Log(componentType);
                GameObject[] allGameObject = FindObjectsOfType<GameObject>();
                foreach (var go in allGameObject)
                {
                    if (go.GetComponent(componentType) != null)
                    {
                        targetObject = go;
                        break;
                    }
                }
                if (targetObject)
                    objComponent = targetObject.GetComponent(componentType);
            }

            if (parameterTypes == null)
            {
                parameterTypes = GetParameterTypes();
            }

            if (objMethod == null)
            {
                objMethod = UnityEvent.GetValidMethodInfo(objComponent, targetMethod, parameterTypes);
            }
        }
        private void Start()
        {
            if (componentType == null)
            {
                componentType = Type.GetType(mytargetComponentAssemblyName);
            }
            if (objComponent == null)
            {
                Debug.Log(componentType);
                targetObject = FindObjectOfType(componentType) as GameObject;
                Debug.Log(targetObject);
                objComponent = targetObject.GetComponent(componentType);
            }

            if (parameterTypes == null)
            {
                parameterTypes = GetParameterTypes();
            }

            if (objMethod == null)
            {
                objMethod = UnityEvent.GetValidMethodInfo(objComponent, targetMethod, parameterTypes);
            }
        }
        public override Color GetButtonColor()
        {
            return new Color(11, 34, 34, 222);
        }
        public override string GetSummary()
        {
            if (targetObject == null && string.IsNullOrEmpty(mytargetComponentAssemblyName))
            {
                return "Error: targetObject is not assigned";
            }

            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }
            if (!string.IsNullOrEmpty(mytargetComponentAssemblyName))
            {
                string[] temp = mytargetComponentAssemblyName.Split('.');
                return temp[temp.Length-1];
            }
            return targetObject.name + "." + targetComponentText + "." + targetMethodText;
        }
    }
}
