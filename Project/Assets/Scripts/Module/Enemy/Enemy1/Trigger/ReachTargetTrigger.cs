using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 目标进入攻击范围
    /// </summary>
    public class ReachTargetTrigger : FsmTrigger<FsmEnemy>
    {
        public override bool OnTriggerHandler()
        {
            return Fsm.target != null && Vector3.Distance(Fsm.target.transform.position, Fsm.transform.position) < Fsm.atkDistance;
        }
    }
}