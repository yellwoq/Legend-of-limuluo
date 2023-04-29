using System;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 动画事件行为助手
    /// </summary>
    public class AnimationEventBehaviour : MonoBehaviour
    {

        private Animator anim;
        /// <summary>
        /// 攻击时绑定的事件
        /// </summary>
        public event Action AttackHandler;

        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// 撤销动画播放 complete
        /// </summary>
        /// <param name="animParam">撤销的动画参数名称</param>
        public void OnCancelAnim(string animParam)
        {
            anim.SetBool(animParam, false);
        }

        /// <summary>
        /// 攻击时执行
        /// </summary>
        public void OnAttack()
        {
            if (AttackHandler != null)
                AttackHandler();
        }
    }
}