using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    public class DeadState : FsmState<FsmEnemy>
    {
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Fsm.anim.SetBool(Fsm.status.chParams.death, true);
        } 
    }
}