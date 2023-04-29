using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 击杀目标
    /// </summary>
    public class KilledTargetTrigger : FsmTrigger<FsmEnemy>
    {
        public override bool OnTriggerHandler()
        {
            return Fsm.target == null || Fsm.target.currentHP == 0;
        }
    }
}