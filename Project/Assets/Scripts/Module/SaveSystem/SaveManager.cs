using Bag;
using Common;
using MapSystem;
using MVC;
using QuestSystem;
using SkillSystem;
using StorySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SaveSystem
{
    /// <summary>
    /// 存档管理器
    /// </summary>
    public class SaveManager : MonoSingleton<SaveManager>
    {
        /// <summary>
        /// 在加载时不要销毁
        /// </summary>
        public static bool DontDestroyOnLoadOnce;
        /// <summary>
        /// 非英雄数据文件路径
        /// </summary>
        public string ExceptHeroDataFilePath
        {
            get
            {
                if (GameController.I.crtUser == null || GameController.I.crtHero == null) return null;
                string storePathDir = Application.persistentDataPath + "/" + GameController.I.crtUser.uid + "/" + GameController.I.crtHero.heroId;
                //如果文件夹不存在
                if (!Directory.Exists(storePathDir))
                { Directory.CreateDirectory(storePathDir); }
                //文件路径
                return storePathDir + "/" + exceptHeroDataName;
            }
        }

        public bool IsLoading { get; private set; }
        /// <summary>
        /// 非英雄数据文件名
        /// </summary>
        [DisplayName("非英雄数据文件名", true)]
        public string exceptHeroDataName;
        /// <summary>
        /// 英雄数据文件路径
        /// </summary>
        public string HeroDataFilePath
        {
            get
            {
                if (GameController.I.crtUser == null || GameController.I.crtHero == null) return null;
                string storePathDir = Application.persistentDataPath + "/" + GameController.I.crtUser.uid + "/" + GameController.I.crtHero.heroId;
                //如果文件夹不存在
                if (!Directory.Exists(storePathDir))
                    Directory.CreateDirectory(storePathDir);
                //文件路径
                return storePathDir + "/" + heroDataName;
            }
        }
        /// <summary>
        /// 英雄数据文件名
        /// </summary>
        [DisplayName("英雄数据文件名", true)]
        public string heroDataName;

        private void Start()
        {
            if (!DontDestroyOnLoadOnce)
            {
                DontDestroyOnLoad(this);
                DontDestroyOnLoadOnce = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #region 存档相关

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        public bool Save()
        {
            FileStream fs = OpenFile(ExceptHeroDataFilePath, FileMode.Create);
            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                SaveData data = new SaveData();
                //保存背包
                SaveBag(data);
                //保存技能
                SaveSkill(data);
                //保存玩家任务
                SavePlayerQuest(data);
                //保存剧情
                SaveStory(data);
                //保存地图
                SaveMap(data);
                //保存地图图标数据
                SaveMapMark(data);
                //序列化数据
                bf.Serialize(fs, data);
                fs.Close();
                Components.Alert.Show("保存成功", "目前进度已经保存！");
                return true;
            }
            catch (Exception ex)
            {
                if (fs != null) fs.Close();
                Debug.LogError(ex.Message);
                return false;
            }
        }
        /// <summary>
        /// 保存剧情数据
        /// </summary>
        /// <param name="data"></param>
        void SaveStory(SaveData data)
        {
            foreach (var story in StoryManager.I.StoryList)
            {
                data.storyDatas.Add(new StoryData(story));
            }
        }
        /// <summary>
        /// 保存地图数据
        /// </summary>
        /// <param name="data"></param>
        void SaveMap(SaveData data)
        {
            foreach (var map in MapAgentManager.I.MapList)
            {
                data.mapDatas.Add(new MapData(map));
            }
        }
        /// <summary>
        /// 保存地图图标数据
        /// </summary>
        /// <param name="data"></param>
        void SaveMapMark(SaveData data)
        {
            MapManager.Instance.SaveData(data);
        }
        /// <summary>
        /// 保存背包数据
        /// </summary>
        /// <param name="data"></param>
        void SaveBag(SaveData data)
        {
            foreach (KeyValuePair<string, string> equipItem in BagPanel.I.equipMap)
            {
                data.equipItemDatas.Add(new EquipItemData(equipItem.Key, equipItem.Value));
            }
            foreach (KeyValuePair<int, int> itemList in BagPanel.I.Items)
            {
                data.itemDatas.Add(new ItemData(itemList.Key.ToString(), itemList.Value));
            }
        }
        /// <summary>
        /// 保存技能
        /// </summary>
        /// <param name="data"></param>
        void SaveSkill(SaveData data)
        {
            foreach (var skill in SkillManager.I.allSkills)
            {
                data.skillDatas.Add(new SkillData(skill));
            }
            data.skillPoint = SkillManager.I.skillpoint;
        }
        /// <summary>
        /// 保存任务数据
        /// </summary>
        /// <param name="data"></param>
        void SavePlayerQuest(SaveData data)
        {
            foreach (Quest quest in QuestManager.Instance.QuestsOngoing)
            {
                data.ongoingQuestDatas.Add(new QuestData(quest));
            }
            foreach (Quest quest in QuestManager.Instance.QuestsComplete)
            {
                data.completeQuestDatas.Add(new QuestData(quest));
            }
        }
        /// <summary>
        /// 保存英雄数据
        /// </summary>
        /// <param name="heroData"></param>
        public void SaveHeroData(HeroStateData heroData, string FilePath = null)
        {
            SendNotification(NotiList.CHANGE + NotiList.USER_HERO_DATA, heroData.heroAttrData);
            //用Json数据类型存储英雄相关数据
            string heroDataStr = JsonUtility.ToJson(heroData);
            if (FilePath == null)
                File.WriteAllText(HeroDataFilePath, heroDataStr, Encoding.UTF8);
            else
            {
                if (Directory.Exists(Path.GetDirectoryName(FilePath)))
                    File.WriteAllText(HeroDataFilePath, heroDataStr, Encoding.UTF8);
                else
                    throw new System.Exception("文件路径不存在");
            }
        }
        #endregion

        #region 读档相关
        /// <summary>
        /// 加载
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            using (FileStream fs = OpenFile(ExceptHeroDataFilePath, FileMode.Open))
            {
                try
                {

                    BinaryFormatter bf = new BinaryFormatter();

                    SaveData data = bf.Deserialize(fs) as SaveData;
                    fs.Close();
                    // 显示进度条
                    Components.Loading.I.Show();
                    StartCoroutine(LoadAsync(data));
                    return true;
                }

                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                    return false;
                }
            }
        }
        /// <summary>
        /// 异步加载
        /// </summary>
        /// <returns></returns>
        IEnumerator LoadAsync(SaveData data)
        {
            IsLoading = true;
            Sound.SoundManager.I.PlayBgm("BGM2");
            StartCoroutine(SceneLoader.I.UPDateProgressSlider(LoadHeroData().sceneName));
            yield return new WaitUntil(() => SceneLoader.I.ao.isDone);
            Components.Loading.I.Hide();
            try
            {
                UIManager.I.TogglePanel(Panels.MainPanel, false);
                GameManager.Instance.Init();
                SendNotification(NotiList.GET_ITEM_MAP);
                //读取背包
                LoadBag(data);
                //读取技能
                LoadSkill(data);
                //读取地图数据
                LoadMap(data);
                //读取地图图标数据
                LoadMapMark(data);
                //读取剧情信息
                LoadStory(data);
                //读取玩家任务
                LoadPlayerQuest(data);
                IsLoading = false;
            }
            catch
            {
                StopCoroutine(LoadAsync(data));
                throw;
            }
        }
        /// <summary>
        /// 读取英雄数据
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public HeroStateData LoadHeroData(string filepath = null)
        {
            string dataStr;
            if (HeroDataFilePath == null) return null;
            if (filepath != null)
                dataStr = File.ReadAllText(filepath, Encoding.UTF8);
            else
                dataStr = File.ReadAllText(HeroDataFilePath, Encoding.UTF8);
            HeroStateData playerHeroData = JsonUtility.FromJson<HeroStateData>(dataStr);
            return playerHeroData;
        }
        /// <summary>
        /// 读取玩家任务
        /// </summary>
        /// <param name="data"></param>
        void LoadPlayerQuest(SaveData data)
        {
            QuestManager.I.QuestsOngoing.Clear();
            foreach (QuestData questData in data.ongoingQuestDatas)
            {
                HandlingQuestData(questData);
                QuestManager.Instance.UpdateObjectivesText();
            }
            QuestManager.I.QuestsComplete.Clear();
            foreach (QuestData questData in data.completeQuestDatas)
            {
                Quest quest = HandlingQuestData(questData);
                QuestManager.Instance.CompleteQuest(quest);
            }
        }
        /// <summary>
        /// 读取背包
        /// </summary>
        /// <param name="data"></param>
        void LoadBag(SaveData data)
        {
            foreach (ItemData itemData in data.itemDatas)
            {
                foreach (EquipItemData equipItemData in data.equipItemDatas)
                {
                    if (itemData.itemID == equipItemData.itemID)
                    {
                        itemData.itemAmount--;
                        //装备物品显示
                        GameObject itemGO = Instantiate(BagPanel.I.bagItemPrefab, BagPanel.I.transform.FindChildByName(equipItemData.boxName).transform);
                        itemGO.transform.localPosition = Vector3.zero;
                        itemGO.GetComponent<BagItem>().SetItem(Convert.ToInt32(itemData.itemID), 1);
                        break;
                    }
                }
                BagPanel.Instance.GetItem(int.Parse(itemData.itemID), itemData.itemAmount);
            }
        }
        /// <summary>
        /// 读取技能
        /// </summary>
        /// <param name="data"></param>
        void LoadSkill(SaveData data)
        {
            foreach (SkillData skillData in data.skillDatas)
            {
                foreach (var skill in SkillManager.I.allSkills)
                {
                    //如果是同一个技能
                    if (skill.SkillID == skillData.skillID)
                    {
                        skill.Level = skillData.skillLevel;
                        Debug.Log(skill.SkillName + skillData.applySkill);
                        PlayerPrefs.SetString(skillData.applySkill, skillData.skillID);
                        break;
                    }

                }
            }
            SkillManager.I.skillpoint = data.skillPoint;
        }
        /// <summary>
        /// 读取地图
        /// </summary>
        /// <param name="data"></param>
        void LoadMap(SaveData data)
        {
            foreach (MapData mapData in data.mapDatas)
            {
                MapAgentManager.I.SetMap(mapData.mapID, mapData.IsOnGround);
            }
        }
        /// <summary>
        /// 读取地图图标数据
        /// </summary>
        /// <param name="data"></param>
        void LoadMapMark(SaveData data)
        {
            MapManager.Instance.LoadData(data);
        }
        /// <summary>
        /// 读取剧情
        /// </summary>
        /// <param name="data"></param>
        void LoadStory(SaveData data)
        {
            foreach (StoryData storyData in data.storyDatas)
            {
                StoryManager.I.SetStory(storyData.storyID, storyData.HasReaded);
            }
        }
        /// <summary>
        /// 处理任务数据
        /// </summary>
        /// <param name="questData"></param>
        /// <returns></returns>
        Quest HandlingQuestData(QuestData questData)
        {
            QuestGiver questGiver = GameManager.Instance.AllQuestGiver[questData.originGiverID] as QuestGiver;
            Quest quest = questGiver.QuestInstances.Find(x => x._ID == questData.questID);
            quest.isFirstAcceted = false;
            QuestManager.Instance.AcceptQuest(quest);
            foreach (ObjectiveData od in questData.objectiveDatas)
            {
                foreach (Objective o in quest.Objectives)
                {
                    if (o.runtimeID == od.runtimeID)
                    {
                        o.CurrentAmount = od.currentAmount;
                        break;
                    }
                }
            }
            return quest;
        }
        #endregion
        /// <summary>
        /// 用文件流形式打开文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="fileMode">打开方式</param>
        /// <returns></returns>
        FileStream OpenFile(string path, FileMode fileMode)
        {
            try
            {
                return new FileStream(path, fileMode);
            }
            catch
            {
                return null;
            }
        }
    }
}
