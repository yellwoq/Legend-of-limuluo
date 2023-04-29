using Common;
using Enemy;
using QuestSystem;
using System.Collections.Generic;
using UnityEngine;
using MapSystem;
using StorySystem;
using Bag;
using SkillSystem;
using Player;

namespace SaveSystem
{
    /// <summary>
    /// 用来在运行时，间接存储游戏世界所有互动对象的
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        private Dictionary<string, List<EnemyStatus>> allEnermy = new Dictionary<string, List<EnemyStatus>>();
        /// <summary>
        /// 所有的敌人
        /// </summary>
        public Dictionary<string, List<EnemyStatus>> AllEnermy
        {
            get
            {
                allEnermy.Clear();
                EnemyStatus[] enermies = FindObjectsOfType<EnemyStatus>(true);
                foreach (EnemyStatus enermy in enermies)
                {
                    if (!allEnermy.ContainsKey(enermy.enemyID.ToString()))
                    {
                        allEnermy.Add(enermy.enemyID.ToString(), new List<EnemyStatus>());
                    }
                    allEnermy[enermy.enemyID.ToString()].Add(enermy);
                }
                return allEnermy;
            }
        }
        private Dictionary<string, QuestGiver> allQuestGiver = new Dictionary<string, QuestGiver>();
        /// <summary>
        /// 所有的带任务的NPC
        /// </summary>
        public Dictionary<string, QuestGiver> AllQuestGiver
        {
            get
            {
                allQuestGiver.Clear();
                QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>(true);
                foreach (QuestGiver giver in questGivers)
                {
                    try
                    {
                        giver.InitInfo();
                        allQuestGiver.Add(giver.ID, giver);
                    }
                    catch
                    {
                        Debug.LogWarningFormat("[Add quest giver error] ID: {0}  Name: {1}", giver.ID, giver.Name);
                    }
                }
                return allQuestGiver;
            }
        }
        private Dictionary<string, QuestPoint> allQuestPoint = new Dictionary<string, QuestPoint>();
        /// <summary>
        /// 所有的任务点
        /// </summary>
        public Dictionary<string, QuestPoint> AllQuestPoint
        {
            get
            {
                allQuestPoint.Clear();
                QuestPoint[] questPoints = FindObjectsOfType<QuestPoint>(true);
                foreach (QuestPoint point in questPoints)
                {
                    try
                    {
                        allQuestPoint.Add(point._ID, point);
                    }
                    catch
                    {
                        Debug.LogWarningFormat("[Add quest point error] ID: {0}", point._ID);
                    }
                }
                return allQuestPoint;
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            foreach (KeyValuePair<string, QuestGiver> kvp in AllQuestGiver)
            {
                kvp.Value.Init();
            }
            PlayerManager.I.Initplayer();
            MapAgentManager.I.InitMap();
            MapManager.I.SetPlayer(PlayerManager.I.playerTrans);
            StoryManager.I.InitStory();
            BagPanel.I.InitBag();
            SkillManager.I.InitSkill();
            QuestManager.I.InitQuest();
        }
    }
}
