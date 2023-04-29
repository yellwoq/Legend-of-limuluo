namespace AI.FSM
{
    public class PursuitState : FsmState<FsmEnemy>
    {
        public override void OnStateEnter()
        {
            base.OnStateEnter();
            Fsm.SwitchState(Fsm.status.chParams.walk, true);
            Fsm.status.moveSpeed =Fsm.speed;
            Fsm.GetComponent<Pathfinding.AIPath>().maxSpeed = Fsm.speed;
        }

        public override void OnStateStay()
        {
            base.OnStateStay();
            if (Fsm.target != null)
            {
                Fsm.Movement(Fsm.target.transform);
                Fsm.SwitchState(Fsm.status.chParams.walk, true);
            }
        }

        public override void OnStateExit()
        {
            base.OnStateExit();
            Fsm.SwitchState(Fsm.status.chParams.walk, false);
            //停止移动
            Fsm.StopMove();
        }
    }
}