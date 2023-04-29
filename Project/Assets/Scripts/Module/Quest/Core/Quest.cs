using Bag;
using DialogueSystem;
using Fungus;
using Player;
using StorySystem;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649
namespace QuestSystem
{
    /// <summary>
    /// 单个任务
    /// </summary>
    [System.Serializable]
    [CreateAssetMenu(fileName = "quest", menuName = "RPG GAME/Quest/new Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField]
        private Sprite defaultSprite;
        public Sprite DefaultSprite => defaultSprite;
        [Header("任务基本信息")]
        [SerializeField]
        private string ID;
        /// <summary>
        /// 任务id
        /// </summary>
        public string _ID => ID;
        /// <summary>
        /// 是否首次接取
        /// </summary>
        [SerializeField, HideInInspector]
        public bool isFirstAcceted = true;
        [SerializeField, TextArea(1, 1)]
        private string title;
        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title => title;
        [TextArea]
        [SerializeField]
        private string description;
        /// <summary>
        /// 任务描述信息
        /// </summary>
        public string Description => description;
        [SerializeField]
        private bool abandonable = true;
        /// <summary>
        /// 是否可放弃
        /// </summary>
        public bool Abandonable => abandonable;
        [SerializeField, NonReorderable]
        private List<QuestAcceptCondition> acceptConditions;
        /// <summary>
        /// 任务接取条件
        /// </summary>
        public List<QuestAcceptCondition> AcceptConditions => acceptConditions;

        [SerializeField]
        private Dialogue beginDialogue;
        /// <summary>
        /// 任务开始的对话
        /// </summary>
        public Dialogue BeginDialogue => beginDialogue;

        [SerializeField]
        private Dialogue ongoingDialogue;
        /// <summary>
        /// 任务进行中的对话
        /// </summary>
        public Dialogue OngoingDialogue => ongoingDialogue;
        [SerializeField]
        private Dialogue completeDialogue;
        /// <summary>
        /// 任务完成时的对话
        /// </summary>
        public Dialogue CompleteDialogue => completeDialogue;
        [SerializeField]
        private Reward questReward;
        /// <summary>
        /// 任务报酬
        /// </summary>
        public Reward MQuestReward => questReward;

        [SerializeField]
        private bool cmpltOnOriginalNPC = true;
        /// <summary>
        /// 任务完成方式：是否交还给原NPC
        /// </summary>
        public bool CmpltOnOriginalNPC => cmpltOnOriginalNPC;

        [SerializeField]
        private string IDOfNPCToComplete;
        /// <summary>
        /// 完成时交付的NPC的id
        /// </summary>
        public string _IDOfNPCToComplete => IDOfNPCToComplete;

        [SerializeField]
        private bool cmpltObjectiveInOrder = false;
        /// <summary>
        /// 是否分先后
        /// </summary>
        public bool CmpltObjectiveInOrder => cmpltObjectiveInOrder;
        [System.NonSerialized]
        private List<Objective> objectives = new List<Objective>();
        /// <summary>
        /// 存储所有目标，在运行时用到，初始化时自动填，不用人为干预，详见QuestGiver类
        /// </summary>
        public List<Objective> Objectives => objectives;

        [SerializeField, NonReorderable]
        private List<CollectObjective> collectObjectives;
        /// <summary>
        /// 所有的收集目标
        /// </summary>
        public List<CollectObjective> CollectObjectives => collectObjectives;

        [SerializeField, NonReorderable]
        private List<KillObjective> killObjectives;
        /// <summary>
        /// 所有的击杀目标
        /// </summary>
        public List<KillObjective> KillObjectives
        {
            get
            {
                return killObjectives;
            }
        }

        [SerializeField, NonReorderable]
        private List<TalkObjective> talkObjectives;
        /// <summary>
        /// 所有的对话目标
        /// </summary>
        public List<TalkObjective> TalkObjectives => talkObjectives;

        [SerializeField, NonReorderable]
        private List<MoveObjective> moveObjectives;
        /// <summary>
        /// 所有的移动目标
        /// </summary>
        public List<MoveObjective> MoveObjectives => moveObjectives;
        /// <summary>
        /// 初始任务给与者NPC
        /// </summary>
        [HideInInspector]
        public QuestGiver MOriginQuestGiver;
        /// <summary>
        /// 当前任务给与者NPC
        /// </summary>
        [HideInInspector]
        public QuestGiver MCurrentQuestGiver;
        /// <summary>
        /// 任务是否正在执行，在运行时用到
        /// </summary>
        [HideInInspector]
        public bool IsOngoing;
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool IsComplete
        {
            get
            {
                foreach (CollectObjective co in collectObjectives)
                    if (!co.IsComplete) return false;
                foreach (KillObjective ko in killObjectives)
                    if (!ko.IsComplete) return false;
                foreach (TalkObjective to in talkObjectives)
                    if (!to.IsComplete) return false;
                foreach (MoveObjective mo in moveObjectives)
                    if (!mo.IsComplete) return false;
                return true;
            }
        }
        /// <summary>
        /// 现在是否可接取
        /// </summary>
        public bool AcceptAble
        {
            get
            {
                foreach (QuestAcceptCondition qac in AcceptConditions)
                {
                    if (!qac.IsEligible) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 判断该任务是否需要某个道具，用于丢弃某个道具时，判断能不能丢
        /// </summary>
        /// <param name="itemID">所需判定的道具</param>
        /// <param name="leftAmount">剩余数量</param>
        /// <returns></returns>
        public bool RequiredItem(string itemID, int leftAmount)
        {
            if (CmpltObjectiveInOrder)
            {
                foreach (Objective o in Objectives)
                {
                    //当目标是收集类目标时才进行判断
                    if (o is CollectObjective && itemID == (o as CollectObjective).ItemID)
                    {
                        //如果该任务完成并且有顺序
                        if (o.IsComplete && o.InOrder)
                        {
                            //如果剩余数量不足以支撑任务完成
                            if (o.Amount > leftAmount)
                            {
                                Objective tempObj = o.NextObjective;
                                //判断是否有后置目标在进行
                                while (tempObj != null)
                                {
                                    //如果后置目标已经有数量了并且排在其后，则不能丢弃
                                    //保证在打破该目标的完成状态时，后置目标不受影响
                                    if (tempObj.CurrentAmount > 0 && tempObj.OrderIndex > o.OrderIndex)
                                    {
                                        return true;
                                    }
                                    tempObj = tempObj.NextObjective;
                                }
                            }
                            //已经达成目标并且剩余的数量大于等于目标要求的数量
                            return false;
                        }
                        //还没有达成目标
                        return false;
                    }
                }
            }
            //目标不分先后
            return false;
        }
    }
    #region 任务报酬
    /// <summary>
    /// 任务报酬
    /// </summary>
    [System.Serializable]
    public class Reward
    {
        [SerializeField]
        private int money;
        public int Money
        {
            get
            {
                return money;
            }
        }

        [SerializeField]
        private int exp;
        public int EXP
        {
            get
            {
                return exp;
            }
        }
        [SerializeField, NonReorderable]
        private List<ItemReward> Itemrewards;
        /// <summary>
        /// 奖励物品
        /// </summary>
        public List<ItemReward> ItemRewards
        {
            get
            {
                return Itemrewards;
            }
        }
    }
    /// <summary>
    /// 物品报酬
    /// </summary>
    [System.Serializable]
    public class ItemReward
    {
        [SerializeField]
        private int itemID;
        public int ItemID { get { return itemID; } }
        [SerializeField]
        private int rewardNum;
        public int RewardNum { get { return rewardNum; } }
    }
    #endregion

    #region 任务条件
    /// <summary>
    /// 任务接收条件
    /// </summary>
    [System.Serializable]
    public class QuestAcceptCondition
    {

        [SerializeField]
        private ConditionType conditionType = ConditionType.None;
        /// <summary>
        /// 接收条件类型
        /// </summary>
        public ConditionType ConditionType
        {
            get
            {
                return conditionType;
            }
        }

        [SerializeField]
        private int level;
        /// <summary>
        /// 等级要求
        /// </summary>
        public int Level
        {
            get
            {
                return level;
            }
        }

        [SerializeField]
        private string IDOfCompleteQuest;
        /// <summary>
        /// 完成任务的id
        /// </summary>
        public string _IDOfCompleteQuest
        {
            get
            {
                return IDOfCompleteQuest;
            }
        }

        [SerializeField]
        private Quest completeQuest;
        /// <summary>
        /// 所需完成的任务
        /// </summary>
        public Quest CompleteQuest
        {
            get
            {
                return completeQuest;
            }
        }

        [SerializeField]
        private string IDOfOwnedItem;
        /// <summary>
        /// 需要拥有的物品id
        /// </summary>
        public string _IDOfOwnedItem
        {
            get
            {
                return IDOfOwnedItem;
            }
        }
        [SerializeField]
        private int OwnedItemNum;
        /// <summary>
        /// 需要拥有的物品数量
        /// </summary>
        public int _OwnedItemNum => OwnedItemNum;
        [SerializeField]
        private string IDofStoryAgent;
        public string _IDofStoryAgent => IDofStoryAgent;
        /// <summary>
        /// 是否满足
        /// </summary>
        public bool IsEligible
        {
            get
            {
                switch (ConditionType)
                {
                    case ConditionType.None:
                        return true;
                    case ConditionType.LevelLargeThen:
                        return PlayerManager.I.heroData.heroAttrData.lv > level;
                    case ConditionType.LevelLessThen:
                        return PlayerManager.I.heroData.heroAttrData.lv < level;
                    case ConditionType.LevelLargeOrEqualsThen:
                        return PlayerManager.I.heroData.heroAttrData.lv >= level;
                    case ConditionType.LevelLessOrEqualsThen:
                        return PlayerManager.I.heroData.heroAttrData.lv <= level;
                    case ConditionType.CompleteQuest:
                        if (_IDOfCompleteQuest != string.Empty)
                            return QuestManager.Instance.HasCompleteQuestWithID(_IDOfCompleteQuest);
                        else return QuestManager.Instance.HasCompleteQuestWithID(CompleteQuest._ID);
                    case ConditionType.HasItem:
                        if (_IDOfOwnedItem != string.Empty)
                            return BagPanel.Instance.HasItemWithID(_IDOfOwnedItem) && (BagPanel.I.GetItemAmountByID(int.Parse(_IDOfOwnedItem)) == OwnedItemNum);
                        else return false;
                    case ConditionType.StoryPlay:
                        Flowchart needFC = StoryManager.I.StoryList.Find(s => s.FlowChatID == IDofStoryAgent).GetComponent<Flowchart>();
                        return needFC.GetBooleanVariable("isHasRead");
                    default:
                        return true;

                }
            }
        }
    }
    /// <summary>
    /// 条件枚举
    /// </summary>
    public enum ConditionType
    {
        [InspectorName("无条件")]
        None,
        [InspectorName("等级大于")]
        LevelLargeThen,
        [InspectorName("等级小于")]
        LevelLessThen,
        [InspectorName("等级大于或等于")]
        LevelLargeOrEqualsThen,
        [InspectorName("等级小于或等于")]
        LevelLessOrEqualsThen,
        [InspectorName("完成任务")]
        CompleteQuest,
        [InspectorName("拥有物品")]
        HasItem,
        [InspectorName("播放故事")]
        StoryPlay,
    }
    #endregion

    #region 任务目标
    public delegate void ObjectiveStateListner(Objective objective, bool cmpltStateBef);
    [System.Serializable]
    /// <summary>
    /// 任务目标基类
    /// </summary>
    public abstract class Objective
    {
        /// <summary>
        /// 运行时的id
        /// </summary>
        [HideInInspector]
        public string runtimeID;

        [SerializeField]
        private string displayName;
        /// <summary>
        /// 显示的名字
        /// </summary>
        public string DisplayName => displayName;
        [SerializeField]
        private int amount = 1;
        /// <summary>
        /// 要求的数量
        /// </summary>
        public int Amount => amount;
        private int currentAmount;
        /// <summary>
        /// 当前拥有的数量
        /// </summary>
        public int CurrentAmount
        {
            get
            {
                return currentAmount;
            }

            set
            {

                bool befCmplt = IsComplete;
                int befAmount = currentAmount;
                if (value < amount && value >= 0)
                    currentAmount = value;
                else if (value < 0)
                {
                    currentAmount = 0;
                }
                else currentAmount = amount;
                if (befAmount != currentAmount)
                {
                    OnStateChangeEvent?.Invoke(this, befCmplt);
                }
            }
        }

        public bool IsComplete
        {
            get
            {
                if (currentAmount >= amount)
                    return true;
                return false;
            }
        }

        [SerializeField]
        private bool inOrder;
        /// <summary>
        /// 是否有顺序
        /// </summary>
        public bool InOrder => inOrder;

        [SerializeField]
        private int orderIndex;
        /// <summary>
        /// 顺序索引
        /// </summary>
        public int OrderIndex => orderIndex;
        /// <summary>
        /// 前一个目标
        /// </summary>
        [System.NonSerialized]
        public Objective PrevObjective;
        /// <summary>
        /// 下一个目标
        /// </summary>
        [System.NonSerialized]
        public Objective NextObjective;
        /// <summary>
        /// 可并行？
        /// </summary>
        public bool Parallel
        {
            get
            {
                if (!InOrder) return true;//不按顺序，说明可以并行执行
                if (PrevObjective != null && (PrevObjective.OrderIndex == OrderIndex)) return true;//有前置目标，而且顺序码与前置目标相同，说明可以并行执行
                if (NextObjective != null && (NextObjective.OrderIndex == OrderIndex)) return true;//有后置目标，而且顺序码与后置目标相同，说明可以并行执行
                return false;
            }
        }
        public ObjectiveStateListner OnStateChangeEvent;

        /// <summary>
        /// 更新状态
        /// </summary>
        protected virtual void UpdateStatus()
        {
            if (IsComplete) return;
            if (!InOrder) CurrentAmount++;
            else if (InOrder && AllPrevObjCmplt) CurrentAmount++;
        }
        /// <summary>
        /// 判定所有前置目标是否都完成
        /// </summary>
        public bool AllPrevObjCmplt
        {
            get
            {
                Objective tempObj = PrevObjective;
                while (tempObj != null)
                {
                    if (!tempObj.IsComplete && tempObj.OrderIndex < OrderIndex)
                    {
                        return false;
                    }
                    tempObj = tempObj.PrevObjective;
                }
                return true;
            }
        }
        /// <summary>
        /// 判定是否有后置目标正在进行
        /// </summary>
        public bool HasNextObjOngoing
        {
            get
            {
                Objective tempObj = NextObjective;
                while (tempObj != null)
                {
                    if (tempObj.CurrentAmount > 0 && tempObj.OrderIndex > OrderIndex)
                    {
                        return true;
                    }
                    tempObj = tempObj.NextObjective;
                }
                return false;
            }
        }
    }
    /// <summary>
    /// 收集类目标
    /// </summary>
    [System.Serializable]
    public class CollectObjective : Objective
    {
        [SerializeField]
        private string itemID;
        /// <summary>
        /// 物品id
        /// </summary>
        public string ItemID => itemID;

        [SerializeField]
        private bool checkBagAtAccept = true;
        /// <summary>
        /// 用于标识是否在接取任务时检查背包道具看是否满足目标，否则目标重头开始计数
        /// </summary>
        public bool CheckBagAtAccept => checkBagAtAccept;

        [SerializeField]
        private bool loseItemAtSubmit = true;
        /// <summary>
        /// 用于标识是否在提交任务时失去相应道具
        /// </summary>
        public bool LoseItemAtSubmit => loseItemAtSubmit;
        /// <summary>
        /// 在开始时的数量
        /// </summary>
        public int amountWhenStart;
        /// <summary>
        /// 得道具时用到
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="leftAmount">物品剩余数量</param>
        public void UpdateCollectAmountUp(string id, int leftAmount)
        {
            if (id == ItemID)
            {
                if (IsComplete) return;
                Debug.Log(ItemID+"  leftAmount:" +leftAmount);
                //不是按顺序，则数量是接取任务后增加的数量
                if (!InOrder)
                { CurrentAmount = leftAmount - (!checkBagAtAccept ? amountWhenStart : 0); }
                //如果所有前置目标都完成
                else if (AllPrevObjCmplt)
                {
                    CurrentAmount = leftAmount - (!checkBagAtAccept ? amountWhenStart : 0);
                    Debug.Log(ItemID + "  leftAmount:" + CurrentAmount);
                }
            }
        }
        /// <summary>
        /// 丢道具时用到
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="leftAmount"></param>
        public void UpdateCollectAmountDown(string itemID, int leftAmount)
        {
            if (itemID == ItemID)
            {
                //前置目标都完成且没有后置目标在进行时，才允许更新
                if (AllPrevObjCmplt && !HasNextObjOngoing) CurrentAmount = leftAmount;
            }
        }
    }
    /// <summary>
    /// 打怪类目标
    /// </summary>
    [System.Serializable]
    public class KillObjective : Objective
    {
        [SerializeField]
        private string enermyID;
        /// <summary>
        /// 敌人Id
        /// </summary>
        public string EnemyID
        {
            get
            {
                return enermyID;
            }
        }

        public void UpdateKillAmount()
        {
            UpdateStatus();
        }
    }
    /// <summary>
    /// 谈话类目标
    /// </summary>
    [System.Serializable]
    public class TalkObjective : Objective
    {
        [SerializeField]
        private string talkerID;
        /// <summary>
        /// 对话者NPC
        /// </summary>
        public string TalkerID => talkerID;

        [SerializeField]
        private Dialogue dialogue;
        /// <summary>
        /// 对话段
        /// </summary>
        public Dialogue Dialogue => dialogue;
        /// <summary>
        /// 更新对话状态
        /// </summary>
        public void UpdateTalkStatus()
        {
            UpdateStatus();
        }
    }
    /// <summary>
    /// 移动到点类目标
    /// </summary>
    [System.Serializable]
    public class MoveObjective : Objective
    {
        [SerializeField]
        private string pointID;
        /// <summary>
        /// 目标点id
        /// </summary>
        public string PointID
        {
            get
            {
                return pointID;
            }
        }
        /// <summary>
        /// 更新进入目标点状态
        /// </summary>
        /// <param name="point"></param>
        public void UpdateMoveIntoStatus(QuestPoint point)
        {
            if (point._ID == PointID)
                UpdateStatus();
        }
        /// <summary>
        /// 更新离开目标点状态
        /// </summary>
        /// <param name="point"></param>
        public void UpdateMoveAwayStatus(QuestPoint point)
        {
            if (point._ID == PointID && !AllPrevObjCmplt)
                CurrentAmount--;
        }
    }
    #endregion
}
