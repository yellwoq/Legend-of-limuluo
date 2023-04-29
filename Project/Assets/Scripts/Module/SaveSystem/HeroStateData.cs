using MVC;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// 英雄进度存储数据
    /// </summary>
    [System.Serializable]
    public class HeroStateData
    {
        /// <summary>
        /// 当前的主任务标题
        /// </summary>
        [DisplayName("当前主任务标题")]
        public string currentMainQuestTitle;
        /// <summary>
        /// 玩家状态数据
        /// </summary>
        [DisplayName("英雄属性数据")]
        public UserHeroVO heroAttrData;
        /// <summary>
        /// 存档时间
        /// </summary>
        [DisplayName("存档时间")]
        public string saveDate;
        /// <summary>
        /// 玩家相关位置信息
        /// </summary>
        [DisplayName("存档的X轴坐标")]
        public float positionX;
        [DisplayName("存档的Y轴坐标")]
        public float positionY;
        [DisplayName("存档的Z轴坐标")]
        public float positionZ;
        /// <summary>
        /// 存档时的场景名
        /// </summary>
        [DisplayName("存档的场景名")]
        public string sceneName;

        public HeroStateData() { }
        public HeroStateData(UserHeroVO userHeroVO)
        {
            SetHeroSave(userHeroVO);
        }
        /// <summary>
        ///设置英雄存储数据
        /// </summary>
        /// <param name="userHeroVO"></param>
        public void SetHeroSave(UserHeroVO userHeroVO)
        {
            if (heroAttrData == null)
            {
                heroAttrData = new UserHeroVO();
            }
            heroAttrData.userId = userHeroVO.userId;
            heroAttrData.heroId = userHeroVO.heroId;
            heroAttrData.heroName = userHeroVO.heroName;
            heroAttrData.heroType = userHeroVO.heroType;
            heroAttrData.lv = userHeroVO.lv;
            heroAttrData.money = userHeroVO.money;
            //基础属性
            heroAttrData.force = userHeroVO.force;
            heroAttrData.spirit = userHeroVO.spirit;
            heroAttrData.intellect = userHeroVO.intellect;
            heroAttrData.speed = userHeroVO.speed;
            //重要属性
            heroAttrData.DEM = userHeroVO.force * 1.5f + userHeroVO.lv * 0.5f;
            heroAttrData.DEF = userHeroVO.spirit * 1.2f + userHeroVO.lv * 0.5f;
            //成长值相关
            heroAttrData.currentHP = userHeroVO.currentHP;
            heroAttrData.currentMP = userHeroVO.currentMP;
            heroAttrData.currentExp = userHeroVO.currentExp;
            heroAttrData.fileName = userHeroVO.fileName;
            heroAttrData.maxHP = 500 + (userHeroVO.lv - 1) * (100 + userHeroVO.spirit * 2);
            heroAttrData.maxMP = 200 + (userHeroVO.lv - 1) * (100 + (int)(userHeroVO.intellect * 1.5f));
            heroAttrData.nextLvNeedExp = 100 + (userHeroVO.lv - 1) * 100;
            GameController.I.crtHero = heroAttrData;
        }
        public void SetQuest()
        {
            //当前主任务ID存储
            currentMainQuestTitle = QuestSystem.QuestManager.I.QuestsOngoing.Find(e => e.MOriginQuestGiver == GameManager.I.AllQuestGiver["NPC000"]).Title;
        }
        public override string ToString()
        {

            return "当前血量：" + heroAttrData.currentHP + "当前魔量：" + heroAttrData.currentMP + "当前任务" + currentMainQuestTitle;
        }
    }
}
