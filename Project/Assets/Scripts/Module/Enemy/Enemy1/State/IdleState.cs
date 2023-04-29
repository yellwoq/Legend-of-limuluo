using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
namespace AI.FSM
{
    /// <summary>
    /// 闲置状态
    /// </summary>
    public class IdleState : FsmState<FsmEnemy>
    {
        private float waitTime;
        public override void OnStateEnter()
        {
           base.OnStateEnter();
           Fsm.SwitchState(Fsm.status.chParams.idle,true);
            Fsm.isCompleteWait = false;
            waitTime = 0;
        }
        public override void OnStateStay()
        {
            base.OnStateStay();
            waitTime += Time.deltaTime;
            if (waitTime > Fsm.waitTime)
            {
                Fsm.isCompleteWait = true;
                return;
            }
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            Fsm.SwitchState(Fsm.status.chParams.idle,false);
        }
    }
}