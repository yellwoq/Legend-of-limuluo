using MapSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0109
namespace Player
{
    /// <summary>
    /// 用于跟随玩家的相机
    /// </summary>
    public class CameraFollowTarget : MonoBehaviour
    {
        #region 字段
        [DisplayName("要跟随的游戏物体")]
        public Transform target;
        [DisplayName("跟随的最小范围")]
        public Vector2 rangeMin;
        [DisplayName("跟随的最大范围")]
        public Vector2 rangeMax;
        [DisplayName("是否使用一定的时间移动过去",false)]
        public bool isFollowWithTime = false;
        [ConditionalHide("延迟的时间", "isFollowWithTime",true)]
        public float delayTime;
        [DisplayName("计时器",true)]
        public float timer;
        [DisplayName("地图范围")]
        public MapRange mapRange;
        private Vector3 startPos;// 开始的位置

        private bool isChangeView;// 是否修改View

        [HideInInspector]
        public float defaultView;//默认范围
        private float currentView;//当前范围
        private float targetView;//目标范围
        private Transform defaultTarget;
        new Camera camera;
        #endregion
        private void Start()
        {
            target = PlayerManager.I.playerTrans;
            camera = GetComponent<Camera>();
            //默认的视野
            defaultView = camera.fieldOfView;
        }

        private void LateUpdate()
        {
            Follow();
        }
        /// <summary>
        /// 跟随玩家
        /// </summary>
        public void Follow()
        {
            if (target == null)
            {
                Debug.LogWarning(transform.name + " 要跟随的目标为空! ");
                return;
            }
            if (mapRange != null)
            {
                camera.orthographicSize = 2;
                rangeMin = mapRange.RangeMin;
                rangeMax = mapRange.RangeMax;
            }
            //跟随的位置
            Vector3 targetPos;
            //如果是以一定的范围跟过去
            if (isFollowWithTime)
            {
                timer += Time.deltaTime;
                targetPos = Vector3.Lerp(startPos, target.position, timer / delayTime);
                targetPos.z = transform.position.z;
                //如果正在改变视野
                if (isChangeView)
                {
                    camera.fieldOfView = Mathf.Lerp(currentView, targetView, timer / delayTime);

                }
                //如果延误时间大于1
                if (timer / delayTime > 1)
                {
                    isChangeView = false;
                    isFollowWithTime = false;
                }

            }
            else
            {
                targetPos = new Vector3(target.position.x, target.position.y, transform.position.z);
            }

            transform.position = LimitPos(targetPos);



        }

        /// <summary>
        /// 设置跟随的目标
        /// </summary>
        /// <param name="target"></param>
        public void SetFollowTarget(Transform target)
        {
            isFollowWithTime = false;
            this.target = target;
        }
        /// <summary>
        /// 延迟跟随目标
        /// </summary>
        /// <param name="target">目标位置</param>
        /// <param name="time">延迟时间</param>
        public void SetFollowTarget(Transform target, float time)
        {
            this.target = target;
            isFollowWithTime = true;
            timer = 0;
            delayTime = time;
            startPos = transform.position;
        }
        /// <summary>
        /// 延迟跟随目标，设置范围
        /// </summary>
        /// <param name="target"></param>
        /// <param name="view"></param>
        /// <param name="time"></param>
        public void SetFollowTarget(Transform target, float view, float time)
        {
            isChangeView = true;
            targetView = view;
            currentView = camera.fieldOfView;
            SetFollowTarget(target, time);

        }

        /// <summary>
        ///  当物体下落调用 过一段时间 恢复
        /// </summary>
        /// <param name="target"></param>
        /// <param name="time"></param>
        public void SetFollowTarget(GameObject target, int time)
        {
            defaultTarget = this.target; // 对当前相机跟随的目标做一个保存

            SetFollowTarget(target.transform, time);

            Invoke("ResetFollowTarget", 3);
        }
        /// <summary>
        /// 重设跟随的目标
        /// </summary>
        public void ResetFollowTarget()
        {
            Debug.Log(" ResetFollowTarget ");
            SetFollowTarget(defaultTarget, 1);
            // 恢复玩家操作
            //PlayerInput.instance.SetEnable(true);
        }
        /// <summary>
        /// 限制位置
        /// </summary>
        /// <param name="targetPos">目标位置</param>
        /// <returns></returns>
        public Vector3 LimitPos(Vector3 targetPos)
        {
            if (targetPos.x < rangeMin.x)
            {
                targetPos.x = rangeMin.x;
            }

            if (targetPos.y < rangeMin.y)
            {
                targetPos.y = rangeMin.y;
            }

            if (targetPos.x > rangeMax.x)
            {
                targetPos.x = rangeMax.x;
            }

            if (targetPos.y > rangeMax.y)
            {
                targetPos.y = rangeMax.y;
            }
            return targetPos;
        }
    }
}
