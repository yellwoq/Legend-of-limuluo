using Common;
using MVC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    ///  技能信息
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "Skill", menuName = "RPG GAME/Skill/new Skill")]
    public class Skill : ScriptableObject
    {
        [Header("技能基本信息"), DisplayName("技能ID"), SerializeField]
        private string skillID;
        public string SkillID => skillID;
        [SerializeField, DisplayName("技能名")]
        private string skillName;
        public string SkillName => skillName;
        [SerializeField, DisplayName("当前技能等级")]
        private int level = 0;
        public int Level
        {
            get { return level; }
            set { level = value; }
        }
        [SerializeField, DisplayName("最高技能等级")]
        private int maxlevel;
        public int MaxLevel => maxlevel;
        [SerializeField, DisplayName("冷却时间")]
        private int coolingtime;
        public int Coolingtime => coolingtime;
        [DisplayName("是否处于冷却状态"), SerializeField]
        private bool isCoding = false;
        public bool IsCoding
        {
            get { return isCoding; }
            set { isCoding = value; }
        }
        [SerializeField, DisplayName("基础耗蓝量")]
        private int initMpCons;
        public int MpCons => initMpCons + (int)(level * mpConsGrowth);
        [SerializeField, DisplayName("英雄类型", false, true, "战士", "法师")]
        private HeroType heroType;
        public HeroType HeroType => heroType;
        [SerializeField, DisplayName("技能释放特效")]
        private GameObject skillEffect;
        public GameObject SkillEffect => skillEffect;
        [SerializeField, DisplayName("技能图标")]
        private Sprite skillIcon;
        public Sprite SkillIcon => skillIcon;
        [SerializeField, InspectorName("技能描述信息"), TextArea]
        private string description;
        public string Description => description;
        [Header("技能攻击信息"), SerializeField, DisplayName("攻击范围")]
        private float attackRange = 2;
        public float AttackRange => attackRange;
        [SerializeField, DisplayName("基础伤害值")]
        private float initdamage;
        public float Damage => initdamage + level * growthDamage;
        [SerializeField, DisplayName("攻击目标所在层级")]
        private LayerMask targetMask;
        public LayerMask TargetMask => targetMask;
        [SerializeField, DisplayName("伤害延迟时间")]
        private float damageDelay = 0.2f;
        public float DamageDelay => damageDelay;
        [SerializeField, DisplayName("技能回收时间")]
        private float collectDelay = 0.3f;
        public float CollectDelay => collectDelay;
        [Header("成长值"), SerializeField, DisplayName("伤害成长值")]
        private float growthDamage;
        public float GrowthDamage => growthDamage;
        [SerializeField, DisplayName("魔法消耗成长值")]
        private float mpConsGrowth;
        public float MpConsGrowth => mpConsGrowth;
        [Header("技能效果"), SerializeField, DisplayName("技能释放位置集合")]
        private List<Releaseposition> startOffset;
        public List<Releaseposition> StartOffset => startOffset;
        [SerializeField, DisplayName("技能攻击方式", false, true, "原地释放", "直线", "圆形", "扇形", "位移")]
        private SkillAttackMode skillattackMode;
        public SkillAttackMode SkillattackMode => skillattackMode;
        [SerializeField, ConditionalHide("攻击角度", "skillattackMode", (int)(SkillAttackMode.Sector), true)]
        private float attackAngle;
        public float AttackAngle => attackAngle;
        [SerializeField, ConditionalHide("距离", "skillattackMode", (int)~SkillAttackMode.InPlace, true)]
        private float distance;
        public float Distance => distance;
        [SerializeField, ConditionalHide("持续时间", "skillattackMode", (int)(SkillAttackMode.Move | SkillAttackMode.InPlace), true)]
        private float damagetime;
        public float Damagetime => damagetime;
        [SerializeField, ConditionalHide("增益类型", "skillattackMode", (int)(SkillAttackMode.InPlace), true)]
        private Gaintype gainType;
        public Gaintype skillGainType => gainType;
        public override string ToString()
        {
            return "技能名称:" + name + "\n当前技能等级:" + level + "\n耗蓝量:" + (initMpCons + level * mpConsGrowth) + "\n冷却时间:" +
                coolingtime + "\n伤害值:" + (initdamage + level * growthDamage) + "\n描述信息:" + description;
        }
    }
    /// <summary>
    /// 释放位置
    /// </summary>
    [Serializable]
    public class Releaseposition
    {
        [SerializeField, DisplayName("释放方向", false, true, "左", "右", "上", "下")]
        private Direction releaseDir;
        public Direction ReleaseDir => releaseDir;
        [SerializeField, DisplayName("位置偏移")]
        private Vector3 posOffset;
        public Vector2 PosOffset => posOffset;
        [SerializeField, DisplayName("角度旋转")]
        private Vector3 rotateOffset;
        public Vector3 RotateOffset => rotateOffset;
    }

    public enum SkillAttackMode
    {
        InPlace = 1,
        Linear = 2,
        Round = 4,
        Sector = 8,
        Move = 16
    }
    public enum Gaintype
    {
        Damage,
        Hp
    }
}
