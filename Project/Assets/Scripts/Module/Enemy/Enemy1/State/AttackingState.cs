using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 攻击状态
    /// </summary>
    public class AttackingState : FsmState<FsmEnemy>
    {
        private float atkTime;
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Fsm.SwitchState(Fsm.status.chParams.attack, true);
            Fsm.GetComponent<Pathfinding.AIPath>().maxSpeed = 0;
           atkTime = 0;
        }
        public override void OnStateStay()
        {
            base.OnStateStay();
            atkTime += Time.deltaTime;
            if (atkTime > Fsm.atkInterval)
            {
                Fsm.SwitchState(Fsm.status.chParams.attack, true);
                //有目标
                if (Fsm.target)
                {
                    if (Vector3.Distance(Fsm.target.transform.position, Fsm.transform.position) < Fsm.atkDistance)
                    {
                        Fsm.target.TakeDamage(Fsm.status.DEM);
                    }
                }
                atkTime = 0;
                return;
            }
            else
            {
                Fsm.SwitchState(Fsm.status.chParams.attack,false);
                Fsm.SwitchState(Fsm.status.chParams.idle, true);
            }
            
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            Fsm.SwitchState(Fsm.status.chParams.attack, false);
        }
    }
}