using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 系统英雄信息
    /// </summary>
    [System.Serializable,CreateAssetMenu(fileName ="SystemHero",menuName = "RPG GAME/Hero/new SystemHero")]
    public class SystemHero : ScriptableObject
    {
        [SerializeField,DisplayName("英雄ID")]
        private int heroID;
        public int HeroID => heroID;
        [SerializeField,DisplayName("英雄类型")]
        private string heroType;
        public string HeroType => heroType;
        [SerializeField,DisplayName("角色名")]
        private string roleName;
        public string RoleName => roleName;
        [SerializeField,TextArea,Header("描述信息")]
        private string decription;
        public string Decription => decription;
    }
}
