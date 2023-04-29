using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 丢失目标
    /// </summary>
    public class LoseTargetTrigger : FsmTrigger<FsmEnemy>
    {
        public override bool OnTriggerHandler()
        {
            return Fsm.target == null;
        }
    }
}