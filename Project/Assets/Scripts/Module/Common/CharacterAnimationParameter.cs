using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 角色动画参数类：封装角色具有的动画参数。
    /// </summary>
    [System.Serializable]
    public class CharacterAnimationParameter
    {
        [DisplayName("死亡动画参数"), Space]
        public string death="Die";
        [DisplayName("站立动画参数"), Space]
        public string idle="Idle";
        [DisplayName("攻击动画参数"), Space]
        public string attack = "Attack";
        [DisplayName("行走动画参数"), Space]
        public string walk = "Walk"; 
    }
    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }
}