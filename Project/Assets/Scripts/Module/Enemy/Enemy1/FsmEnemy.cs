using Common;
using Enemy;
using Pathfinding;
using Player;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 敌人具体状态机：为具体状态以及条件提供成员
    /// </summary>
    public class FsmEnemy : FsmBase<FsmEnemy>,IResetable
    {
        [HideInInspector]
        public Animator anim;
        /// <summary>
        /// 敌人数据
        /// </summary>
        [HideInInspector]
        public EnemyStatus status;
        /// <summary>
        /// 玩家数据
        /// </summary>
        [DisplayName("目标")]
        public PlayerStatus target;
        [DisplayName("目标标签")]
        public string[] tags;

        [DisplayName("攻击距离")]
        public float atkDistance;

        [DisplayName("攻击间隔")]
        public float atkInterval;
        [DisplayName("巡逻等待时间")]
        public float waitTime;
        [DisplayName("巡逻点")]
        public Transform[] patrolPoints;

        [HideInInspector]
        public bool isCompletePatrol;
        [HideInInspector]
        public bool isCompleteWait;
        [DisplayName("移动速度")]
        public float speed = 3;
        [HideInInspector]
        public AIDestinationSetter destinationSetter;

        private new void Awake()
        {
            InitComponent();
            InitPatrolPoint();
            base.Awake();
        }
        private void Start()
        {
            status.currentHP = status.maxHP;
            GetComponentInChildren<HPCanvas>().slider.maxValue = status.maxHP;
        }
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitComponent()
        {
            anim = GetComponentInChildren<Animator>();
            status = GetComponentInChildren<EnemyStatus>();
            destinationSetter = GetComponentInChildren<AIDestinationSetter>();
        }
        /// <summary>
        /// 初始化巡逻点
        /// </summary>
        void InitPatrolPoint()
        {
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                float changeRange = Random.Range(-3, 4);
                Vector3 newPos = patrolPoints[i].position + new Vector3(changeRange, changeRange, 0);
                if (patrolPoints.FindAll(p => p.position == newPos).Length > 0) { i--; continue; }
                patrolPoints[i].position = newPos;
            }
        }
        private new void Update()
        {
            base.Update();
            SearchTarget();
        }
        /// <summary>
        /// 搜索最近目标
        /// </summary>
        private void SearchTarget()
        {
            List<CharacterStatus> allTarget = new List<CharacterStatus>();
            //根据标签 查找所有目标。
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] tempGoArr = GameObject.FindGameObjectsWithTag(tags[i]);
                CharacterStatus[] tempCsArr = tempGoArr.Select(e => e.GetComponent<CharacterStatus>());
                allTarget.AddRange(tempCsArr);
            }
            //条件：视野内的活的最近的
            allTarget = allTarget.FindAll(e => e.currentHP > 0 && Vector3.Distance(transform.position, e.transform.position) < status.sightDistance);
            target = allTarget.ToArray().GetMin(e => Vector3.Distance(transform.position, e.transform.position)) as PlayerStatus;
        }

        /// 运动(由追逐状态 和 巡逻状态 调用)
        /// </summary>
        /// <param name="target"></param>
        public void Movement(Transform target)
        {
            CaculaterDir(transform.position, target.position);
            destinationSetter.target = target;
        }
        public void CaculaterDir(Vector3 myPos, Vector3 targetPos)
        {
            Vector3 dir = (targetPos - myPos).normalized;
            status.horizontalValue = dir.x;
            status.vertcalValue = dir.y;
        }

        /// <summary>
        /// 停止运动
        /// </summary>
        public void StopMove()
        {
            status.moveSpeed = 0;
        }
        /// <summary>
        /// 切换动画
        /// </summary>
        /// <param name="stateName"></param>
        /// <param name="state"></param>
        public void SwitchState(string stateName, bool state)
        {
            anim.SetBool(status.chParams.idle + Direction.Left, false);
            anim.SetBool(status.chParams.idle + Direction.Up, false);
            float currentAngle = CharacterAnimStateSwitch.CaculaterAngle(status.horizontalValue, status.vertcalValue);
            if (currentAngle < 22.5f && currentAngle >= 0f || currentAngle < 360f && currentAngle >= 315f)//左
            {
                anim.SetBool(stateName + Direction.Up, false);
                GetComponent<SpriteRenderer>().flipX = false;
                anim.SetBool(stateName + Direction.Left, state);

            }
            else if (currentAngle >= 22.5f && currentAngle < 157.5f)//上
            {
                anim.SetBool(stateName + Direction.Left, false);
                if (currentAngle >= 90)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                anim.SetBool(stateName + Direction.Up, state);
            }
            else if (currentAngle >= 157f && currentAngle < 225f)//右
            {
                anim.SetBool(stateName + Direction.Up, false);
                GetComponent<SpriteRenderer>().flipX = true;
                anim.SetBool(stateName + Direction.Left, state);
            }
            else if (currentAngle < 315f && currentAngle >= 225f)//下
            {
                anim.SetBool(stateName + Direction.Up, false);
                if (status.horizontalValue > 0)
                { GetComponent<SpriteRenderer>().flipX = true; }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }
                anim.SetBool(stateName + Direction.Left, state);
            }
        }

        public void OnReset()
        {
            InitDefaultState();
        }
    }
}