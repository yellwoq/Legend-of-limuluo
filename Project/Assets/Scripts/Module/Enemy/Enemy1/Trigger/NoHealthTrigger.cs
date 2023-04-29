using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 没有生命
    /// </summary>
    public class NoHealthTrigger : FsmTrigger<FsmEnemy>
    {
        public override bool OnTriggerHandler()
        {
            return Fsm.status.currentHP <= 0;
        }
    }
}