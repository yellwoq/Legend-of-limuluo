using Common;
using Pathfinding;
using System.Collections;
using UnityEngine;
namespace MapSystem
{
    /// <summary>
    /// 普通任务移动
    /// </summary>
    public class PeopleMove : MonoBehaviour
    {
        [HideInInspector]
        public AIDestinationSetter destinationSetter;
        private Animator animator;
        [SerializeField, DisplayName("移动点")]
        private Transform[] movePoints;
        private float h = 0, v = 0;
        private void Awake()
        {
            destinationSetter = GetComponent<AIDestinationSetter>();
            animator = GetComponent<Animator>();
            StartCoroutine(StartMove(0));
        }
        private void OnEnable()
        {

        }
        IEnumerator StartMove(int index)
        {
            Movement(movePoints[index]);
            yield return new WaitUntil(() => Vector3.Distance(transform.position, movePoints[index].position) < 0.2f);
            yield return new WaitForSeconds(4);
            if (index == movePoints.Length - 1)
            {
                index = 0;
            }
            else
                index++;
            StartCoroutine(StartMove(index));
        }

    /// 运动(由追逐状态 和 巡逻状态 调用)
    /// </summary>
    /// <param name="target"></param>
    public void Movement(Transform target)
    {
        CaculaterDir(transform.position, target.position);
        SwitchState(true);
        destinationSetter.target = target;
    }
    public void CaculaterDir(Vector3 myPos, Vector3 targetPos)
    {
        Vector3 dir = (targetPos - myPos).normalized;
        h = dir.x;
        v = dir.y;
    }
    /// <summary>
    /// 切换动画
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="state"></param>
    public void SwitchState(bool state)
    {
        float currentAngle = CharacterAnimStateSwitch.CaculaterAngle(h, v);
        if (currentAngle < 22.5f && currentAngle >= 0f || currentAngle < 360f && currentAngle >= 315f)//左
        {
            animator.SetTrigger(Direction.Left.ToString());
        }
        else if (currentAngle >= 22.5f && currentAngle < 157.5f)//上
        {
            animator.SetTrigger(Direction.Up.ToString());
        }
        else if (currentAngle >= 157f && currentAngle < 225f)//右
        {
            animator.SetTrigger(Direction.Right.ToString());
        }
        else if (currentAngle < 315f && currentAngle >= 225f)//下
        {
            animator.SetTrigger(Direction.Down.ToString());
        }
    }
}
}