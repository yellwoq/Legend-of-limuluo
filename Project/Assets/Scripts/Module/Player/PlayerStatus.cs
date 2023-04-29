using Common;
using MVC;
using SkillSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// 玩家数据类
    /// </summary>
    [Serializable]
    public class PlayerStatus : CharacterStatus
    {
        [DisplayName("力量", true), Space, Header("玩家基础属性")]
        public float force = 5;
        [DisplayName("体力", true)]
        public float spirit = 5;
        [DisplayName("智力", true)]
        public float intellect = 5;
        [DisplayName("当前法力", true), Header("玩家重要属性")]
        public float currentMP = 100;
        [DisplayName("最大法力", true)]
        public float maxMP = 100;
        [DisplayName("玩家攻击范围")]
        public float attackRange = 3;
        [DisplayName("当前经验值", true), Header("其余属性")]
        public float currentExp = 0;
        [DisplayName("到下一等级所需经验值", true)]
        public float nextLvNeedExp = 100;
        [DisplayName("持有金币", true)]
        public float money = 0;
        [DisplayName("英雄名字", true)]
        public string heroName="梨木洛";
        [DisplayName("英雄类型", true)]
        public HeroType heroType=HeroType.Warrior;

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            UserHeroVO newUserHeroVO = GameController.I.crtHero;
            newUserHeroVO.currentHP = currentHP;
            GameController.I.crtHero = newUserHeroVO;
        }
        public override void Death()
        {
            base.Death();
            UserSkillController usc = FindObjectsOfType<UserSkillController>(true).
                Find(u => u.mGainType == Gaintype.Hp && u.isRelased);
            if (heroType == HeroType.Wizard && usc) return;
            GameFault();
        }
        void GameFault()
        {
            //显示游戏失败界面
            GameObject gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
            gameCanvas.GetComponent<CanvasGroup>().alpha = 0;
            gameCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            FindObjectOfType<GameEndPanel>().Show();
        }
    }
}
