using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 看到目标
    /// </summary>
    public class SawTargetTrigger : FsmTrigger<FsmEnemy>
    {
        public override bool OnTriggerHandler()
        {
            return Fsm.target != null;
        }
    }
}