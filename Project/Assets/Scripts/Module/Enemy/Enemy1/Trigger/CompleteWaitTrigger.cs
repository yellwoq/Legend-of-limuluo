using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AI.FSM
{
    public class CompleteWaitTrigger : FsmTrigger<FsmEnemy>
    {
        public override bool OnTriggerHandler()
        {
            return Fsm.isCompleteWait;
        }
    }
}
