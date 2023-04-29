using Common;
using DialogueSystem;
using Fungus;
using MVC;
using QuestSystem;
using SaveSystem;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player
{
    /// <summary>
    /// 玩家信息管理器,负责管理玩家的信息
    /// </summary>
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        [DisplayName("英雄储存数据", true)]
        public HeroStateData heroData;
        public HeroStateData SetHeroSaveData()
        {
            if (heroData == null) return null;
            if (QuestManager.I.QuestsOngoing.Find(e => { return e.MOriginQuestGiver.ID == "NPC000"; }) != null)
                heroData.currentMainQuestTitle = QuestManager.I.QuestsOngoing.Find(e => { return e.MOriginQuestGiver.ID == "NPC000"; }).Title;
            else
                heroData.currentMainQuestTitle = "";
            heroData.positionX = playerTrans.position.x;
            heroData.positionY = playerTrans.position.y;
            heroData.positionZ = playerTrans.position.z;
            heroData.sceneName = SceneManager.GetActiveScene().name;
            heroData.heroAttrData = GameController.I.crtHero;
            return heroData;
        }
        /// <summary>
        /// 玩家位置
        /// </summary>
        [DisplayName("玩家位置相关")]
        public Transform playerTrans = null;
        public void Initplayer()
        {
            PlayerStatus playerStatus = FindObjectOfType<PlayerStatus>(true);
            if (playerStatus == null) return;
            playerTrans = playerStatus.transform;
            //获取到当前的英雄信息
            heroData = SaveSystem.SaveManager.I.LoadHeroData();
            ResourceManager.Load<TalkerInformation>("玩家").Name = heroData.heroAttrData.heroName;
            SetPlayerStatus();
            playerTrans.GetComponent<Character>().SetStandardText(heroData.heroAttrData.heroName);
            playerTrans.position = new Vector3(heroData.positionX, heroData.positionY, heroData.positionZ);

        }
        /// <summary>
        /// 设置玩家状态数据
        /// </summary>
        public void SetPlayerStatus()
        {
            PlayerStatus playerData = playerTrans.GetComponent<PlayerStatus>();
            UserHeroVO heroVO = heroData.heroAttrData;
            if (playerData != null && heroVO != null)
            {
                playerData.Lv = heroVO.lv;
                playerData.money = heroVO.money;
                playerData.heroName = heroVO.heroName;
                playerData.heroType = (HeroType)Enum.Parse(typeof(HeroType), heroVO.heroType);
                //成长值赋值
                playerData.currentExp = heroVO.currentExp;
                playerData.currentHP = heroVO.currentHP;
                playerData.currentMP = heroVO.currentMP;
                playerData.nextLvNeedExp = heroVO.nextLvNeedExp;
                playerData.maxHP = heroVO.maxHP;
                playerTrans.FindChildComponentByName<HPCanvas>("HPCanvas").
                    transform.FindChildComponentByName<Slider>("Slider").maxValue = heroVO.maxHP;
                playerData.maxMP = heroVO.maxMP;
                //基础属性赋值
                playerData.force = heroVO.force;
                playerData.spirit = heroVO.spirit;
                playerData.moveSpeed = heroVO.speed;
                playerData.intellect = heroVO.intellect;
                //重要属性赋值
                playerData.DEM = heroVO.DEM;
                playerData.DEF = heroVO.DEF;
            }
        }
    }
}