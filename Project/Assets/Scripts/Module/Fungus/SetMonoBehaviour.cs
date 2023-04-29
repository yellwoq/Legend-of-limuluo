using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CommandInfo("MonoBehaviour",
    "SetMonoBehaviour",
    "Set MonoBehaviour from target")]
public class SetMonoBehaviour : Command
{
    public GameObject target;
    public string[] monoName;
    public bool active;
    public override void OnEnter()
    {
        MonoBehaviour[] monoBehaviours = target.GetComponents<MonoBehaviour>();
        for (int i = 0; i < monoBehaviours.Length; i++)
        {
            for (int j = 0; j < monoName.Length; j++)
            {
                Type type = Type.GetType(monoName[j]);
                if(type == monoBehaviours[i].GetType())
                {
                    monoBehaviours[i].enabled = active;
                }
            }
        }
        
        Continue();
    }
    public override string GetSummary()
    {
        if (target == null) return "Error:No MonoBehaviour";
        return target.name;
    }

    public override Color GetButtonColor()
    {
        return new Color32(21, 134, 32, 220);
    }
}
