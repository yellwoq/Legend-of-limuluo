using Fungus;
using MapSystem;
using QuestSystem;
using SkillSystem;
using StorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveSystem
{
    /// <summary>
    /// 存档数据类
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        /// <summary>
        /// 玩家数据
        /// </summary>
        public HeroStateData heroData=new HeroStateData();
        /// <summary>
        /// 物品数据集合
        /// </summary>
        public List<ItemData> itemDatas = new List<ItemData>();
        /// <summary>
        /// 技能数据集合
        /// </summary>
        public List<SkillData> skillDatas = new List<SkillData>();
        /// <summary>
        /// 技能点
        /// </summary>
        public int skillPoint;
        /// <summary>
        /// 装备的物品数据集合
        /// </summary>
        public List<EquipItemData> equipItemDatas = new List<EquipItemData>();
        /// <summary>
        /// 进行时任务数据集合
        /// </summary>
        public List<QuestData> ongoingQuestDatas = new List<QuestData>();
        /// <summary>
        /// 完成的任务数据集合
        /// </summary>
        public List<QuestData> completeQuestDatas = new List<QuestData>();
        /// <summary>
        /// 剧情数据集合
        /// </summary>
        public List<StoryData> storyDatas = new List<StoryData>();
        /// <summary>
        /// 地图数据集合
        /// </summary>
        public List<MapData> mapDatas = new List<MapData>();
        /// <summary>
        /// 地图图标数据集合
        /// </summary>
        public List<MapMarkSaveData> markDatas = new List<MapMarkSaveData>();
    }

    /// <summary>
    /// 物品数据
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        /// <summary>
        /// 物品id
        /// </summary>
        public string itemID;
        /// <summary>
        /// 物品数量
        /// </summary>
        public int itemAmount;

        public ItemData(string id, int amount)
        {
            itemID = id;
            itemAmount = amount;
        }
    }
    /// <summary>
    /// 装备的物品数据
    /// </summary>
    [Serializable]
    public class EquipItemData
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        public string itemID;
        /// <summary>
        /// 装备的盒子
        /// </summary>
        public string boxName;

        public EquipItemData(string boxName, string id)
        {
            this.boxName = boxName;
            itemID = id;
        }

    }
    /// <summary>
    /// 技能数据
    /// </summary>
    [System.Serializable]
    public class SkillData
    {
        [DisplayName("技能ID")]
        public string skillID;
        [DisplayName("技能等级")]
        public int skillLevel;
        /// <summary>
        /// 已经配置在技能释放面板上的技能ID
        /// </summary>
        [DisplayName("技能配置")]
        public string applySkill;
        public SkillData(Skill skill)
        {
            skillID = skill.SkillID;
            skillLevel = skill.Level;
            for (int i = 1; i <= 3; i++)
            {
                if (PlayerPrefs.GetString("skill" + i) == skillID)
                {
                    applySkill = "skill" + i;
                    return;
                }
            }
        }
    }
    /// <summary>
    /// 故事剧情
    /// </summary>
    [Serializable]
    public class StoryData
    {
        /// <summary>
        /// 故事ID
        /// </summary>
        public string storyID;
        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool HasReaded;
        public StoryData(StoryAgent storyAgent)
        {
            storyID = storyAgent.FlowChatID;
            Flowchart currentFC = storyAgent.CurrentFlowchart;
            HasReaded = currentFC.GetBooleanVariable("isHasRead");
        }

    }
    /// <summary>
    /// 地图数据
    /// </summary>
    [Serializable]
    public class MapData
    {
        /// <summary>
        /// 地图ID
        /// </summary>
        public string mapID;
        /// <summary>
        /// 是否在该区域上
        /// </summary>
        public bool IsOnGround;
        public MapData(MapAgent map)
        {
            mapID = map.MapID;
            IsOnGround = map.MyTileMap.gameObject.activeSelf;
        }

    }
    /// <summary>
    /// 地图图标数据
    /// </summary>
    [Serializable]
    public class MapMarkSaveData
    {
        public float worldPosX;
        public float worldPosY;
        public float worldPosZ;
        public bool keepOnMap;
        public bool removeAble;
        public string textToDisplay;

        public MapMarkSaveData(MapManager.MapIconWithoutHolder iconWoH)
        {
            worldPosX = iconWoH.worldPosition.x;
            worldPosY = iconWoH.worldPosition.y;
            worldPosZ = iconWoH.worldPosition.z;
            keepOnMap = iconWoH.keepOnMap;
            removeAble = iconWoH.removeAble;
            textToDisplay = iconWoH.textToDisplay;
        }
    }
    /// <summary>
    /// 击杀的Boss数据
    /// </summary>
    [System.Serializable]
    public class KillBossData
    {
        public string bossID;
        public string bossName;
        public bool isKilled;
        public KillBossData()
        {

        }

    }
    /// <summary>
    /// 任务数据
    /// </summary>
    [System.Serializable]
    public class QuestData
    {
        /// <summary>
        /// 任务id
        /// </summary>
        public string questID;
        /// <summary>
        /// 任务原始给予者NPC的id
        /// </summary>
        public string originGiverID;
        /// 目标集合
        /// </summary>
        public List<ObjectiveData> objectiveDatas = new List<ObjectiveData>();

        public QuestData(Quest quest)
        {
            questID = quest._ID;
            originGiverID = quest.MOriginQuestGiver.ID;
            foreach (Objective o in quest.Objectives)
            {
                objectiveDatas.Add(new ObjectiveData(o));
            }
        }
    }
    /// <summary>
    /// 单个目标数据
    /// </summary>
    [System.Serializable]
    public class ObjectiveData
    {
        /// <summary>
        /// ID
        /// </summary>
        public string runtimeID;
        /// <summary>
        /// 当前的数量
        /// </summary>
        public int currentAmount;

        public ObjectiveData(Objective objective)
        {
            runtimeID = objective.runtimeID;
            currentAmount = objective.CurrentAmount;
        }
    }
}