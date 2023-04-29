using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 巡逻状态
    /// </summary>
    public class PatrollingState : FsmState<FsmEnemy>
    {
        private int index;
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Fsm.SwitchState(Fsm.status.chParams.walk, true);
            Fsm.isCompletePatrol = false;
            Fsm.status.moveSpeed = Fsm.speed;
            Fsm.GetComponent<Pathfinding.AIPath>().maxSpeed = Fsm.speed;
            Fsm.destinationSetter.target = null;
            index = 0;
        }
        public override void OnStateStay()
        {
            base.OnStateStay();
           
            if (Vector3.Distance(Fsm.transform.position, Fsm.patrolPoints[index].position) < 0.2f)
            {
                if (index == Fsm.patrolPoints.Length - 1)
                {
                    Fsm.isCompletePatrol = true;
                    index = 0;
                    return;
                }
                else
                    index++;
            }
             Fsm.Movement(Fsm.patrolPoints[index]);
            Fsm.SwitchState(Fsm.status.chParams.walk, true);
        }
        public override void OnStateExit()
        {
            base.OnStateExit();
            Fsm.SwitchState(Fsm.status.chParams.walk, false);
        }
    }
}
