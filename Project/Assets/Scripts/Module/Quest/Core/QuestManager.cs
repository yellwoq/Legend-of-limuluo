using Bag;
using Common;
using Components;
using DialogueSystem;
using Enemy;
using MapSystem;
using MVC;
using Player;
using SaveSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS0649
namespace QuestSystem
{
    /// <summary>
    /// 任务管理,负责任务的管理
    /// </summary>
    public class QuestManager : SingletonMonoBehaviour<QuestManager>
    {
        /// <summary>
        /// 任务预制体
        /// </summary>
        [SerializeField]
        private GameObject questPrefab;
        [DisplayName("任务图标预制体")]
        public QuestFlag QuestFlagsPrefab;
        /// <summary>
        /// 任务列表父物体
        /// </summary>
        [SerializeField]
        public Transform questListParent;
        [SerializeField]
        private Toggle WindowsTog;
        /// <summary>
        /// 任务窗口
        /// </summary>
        [SerializeField]
        private CanvasGroup questsWindow;
        /// <summary>
        /// 任务描述窗口
        /// </summary>
        [SerializeField, Space, Header("任务详情UI相关")]
        private CanvasGroup descriptionWindow;
        public CanvasGroup DescriptionWindow
        {
            get
            {
                return descriptionWindow;
            }
        }
        /// <summary>
        /// 描述信息文本
        /// </summary>
        [SerializeField]
        private Text descriptionText;
        /// <summary>
        /// 放弃按钮
        /// </summary>
        [SerializeField]
        private Button abandonButton;
        /// <summary>
        /// 关闭描述界面按钮
        /// </summary>
        [SerializeField]
        private Button closeDesButton;
        /// <summary>
        /// 金币金钱文本
        /// </summary>
        [SerializeField]
        private Text moneyText;
        [SerializeField]
        private Text expText;
        /// <summary>
        /// 物品奖励集合
        /// </summary>
        [SerializeField]
        private BagItemGrid[] rewardGrids;
        /// <summary>
        /// 任务集合
        /// </summary>
        private List<QuestAgent> QuestAgents = new List<QuestAgent>();

        [SerializeField, Space, ReadOnly, Header("任务列表")]
        private List<Quest> questsOngoing = new List<Quest>();
        /// <summary>
        /// 正在进行的任务集合
        /// </summary>
        public List<Quest> QuestsOngoing
        {
            get
            {
                return questsOngoing;
            }
        }

        [SerializeField, ReadOnly]
        private List<Quest> questsComplete = new List<Quest>();
        /// <summary>
        /// 已经完成的任务集合
        /// </summary>
        public List<Quest> QuestsComplete
        {
            get
            {
                return questsComplete;
            }
        }

        public List<Quest> allQuests;
        /// <summary>
        /// 选择的任务
        /// </summary>
        private Quest SelectedQuest;
        [DisplayName("任务目标图标")]
        public Sprite questIcon;
        public delegate void QuestStatusListener();
        /// <summary>
        /// 任务状态改变时事件监听
        /// </summary>
        public event QuestStatusListener OnQuestStatusChange;
        /// <summary>
        /// 任务图标集合
        /// </summary>
        private readonly Dictionary<Objective, MapIcon> questIcons = new Dictionary<Objective, MapIcon>();

        CanvasGroup gameCanvas;
        #region 任务处理相关
        public void InitQuest()
        {
            foreach (var quest in questsOngoing)
            {
                foreach (Objective o in quest.Objectives)
                    RemoveObjectiveMapIcon(o);
            }
            for (int i = 0; i < questListParent.childCount; i++)
            {
                Destroy(questListParent.GetChild(i).gameObject);
            }
            questsOngoing.Clear();
            questsComplete.Clear();
            QuestAgents.Clear();
            allQuests = ResourceManager.LoadAll<Quest>();
            abandonButton.onClick.AddListener(AbandonSelectedQuest);
            closeDesButton.onClick.AddListener(CloseDescriptionWindow);
            WindowsTog.onValueChanged.AddListener((isOn) => { if (isOn) { OpenQuestWindow(); } else { CloseQuestWindow(); } });
            gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<CanvasGroup>();
        }
        /// <summary>
        /// 接取任务
        /// </summary>
        /// <param name="quest">要接取的任务</param>
        public bool AcceptQuest(Quest quest)
        {
            if (!quest) return false;
            //如果已经存在该任务
            if (HasOngoingQuest(quest)) return false;
            //如果是初次接取任务
            if (quest.isFirstAcceted)
            {
                Sound.SoundManager.I.PlaySfx("AcceptQuest", Sound.SoundManager.ReadAudioClipType.ResourcesLoad);
                PlayerManager.I.playerTrans.gameObject.SetActive(false);
                Alert.Show("新任务", quest.Description, (o) => { PlayerManager.I.playerTrans.gameObject.SetActive(true); },false);
            }
            //产生一个处理任务
            QuestAgent qa = Instantiate(questPrefab, questListParent).GetComponentInChildren<QuestAgent>();
            //属性赋初值
            qa.MQuest = quest;
            StringBuilder @string = new StringBuilder();
            if (quest.MOriginQuestGiver.ID == "NPC000") { @string.Append("<size=17><color=yellow>主任务</color></size>"); }
            @string.Append(quest.Title);
            qa.TitleText.text = @string.ToString();
            QuestAgents.Add(qa);
            //给各个目标赋值
            foreach (Objective o in quest.Objectives)
            {
                o.OnStateChangeEvent += OnObjectiveStateChange;
                //如果是收集目标
                if (o is CollectObjective co)
                {
                    BagPanel.Instance.OnGetItemEvent += co.UpdateCollectAmountUp;
                    BagPanel.Instance.OnLoseItemEvent += co.UpdateCollectAmountDown;
                    //如果是检查背包且非读档模式
                    if (co.CheckBagAtAccept && !SaveManager.Instance.IsLoading)
                        co.UpdateCollectAmountUp(co.ItemID, BagPanel.Instance.GetItemAmountByID(int.Parse(co.ItemID)));
                    else if (!co.CheckBagAtAccept && !SaveManager.Instance.IsLoading)
                        co.amountWhenStart = BagPanel.Instance.GetItemAmountByID(int.Parse(co.ItemID));
                }
                //如果是击杀目标
                else if (o is KillObjective ko)
                {
                    try
                    {
                        foreach (EnemyStatus enermy in GameManager.Instance.AllEnermy[ko.EnemyID])
                            enermy.OnDeathEvent += ko.UpdateKillAmount;
                    }
                    catch
                    {
                        Debug.LogWarningFormat("[找不到敌人] ID: {0}", ko.EnemyID);
                        continue;
                    }
                }
                //如果是对话目标
                else if (o is TalkObjective to)
                {
                    try
                    {
                        if (!o.IsComplete)
                        {
                            var talker = GameManager.Instance.AllQuestGiver[to.TalkerID];
                            talker.talkToThisObjectives.Add(new TalkObjectiveInfo(quest.Title, to));
                            o.OnStateChangeEvent += talker.TryRemoveObjective;
                        }
                    }
                    catch
                    {
                        Debug.LogWarningFormat("[找不到NPC] ID: {0}", to.TalkerID);
                        continue;
                    }
                }
                //如果是移动目标
                else if (o is MoveObjective mo)
                {
                    try
                    {
                        GameManager.Instance.AllQuestPoint[mo.PointID].OnMoveIntoEvent += mo.UpdateMoveIntoStatus;
                        GameManager.Instance.AllQuestPoint[mo.PointID].OnMoveAwayEvent += mo.UpdateMoveAwayStatus;
                    }
                    catch
                    {
                        Debug.LogWarningFormat("[找不到任务点] ID: {0}", mo.PointID);
                        continue;
                    }
                }
            }
            quest.IsOngoing = true;
            QuestsOngoing.Add(quest);
            if (!quest.CmpltOnOriginalNPC)
                GameManager.I.AllQuestGiver[quest._IDOfNPCToComplete].TransferQuestToThis(quest);
            Objective firstObj = quest.Objectives[0];
            CreateObjectiveMapIcon(firstObj);
            OnQuestStatusChange?.Invoke();
            return true;
        }
        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="quest">要放弃的任务</param>
        /// <param name="loadMode">是否读档模式</param>
        /// <returns>是否成功完成任务</returns>
        public bool CompleteQuest(Quest quest)
        {
            if (!quest) return false;
            if (HasOngoingQuest(quest) && quest.IsComplete)
            {
                if (!SaveManager.Instance.IsLoading)
                {
                    List<Quest> questsReqThisQuestItem = new List<Quest>();
                    foreach (Objective o in quest.Objectives)
                    {
                        if (o is CollectObjective co)
                        {
                            questsReqThisQuestItem = QuestsRequireItem(co.ItemID, BagPanel.Instance.GetItemAmountByID(int.Parse(co.ItemID)) - o.Amount).ToList();
                        }
                        //需要道具的任务群包含该任务且数量多于一个，说明有其他任务对该任务需提交的道具存在依赖
                        if (questsReqThisQuestItem.Contains(quest) && questsReqThisQuestItem.Count > 1)
                        {
                            Alert.Show("提交失败！", "提交失败！其他任务对该任务需提交的物品存在依赖");
                            return false;
                        }
                    }
                }
                quest.IsOngoing = false;
                QuestsOngoing.Remove(quest);
                quest.MCurrentQuestGiver.QuestInstances.Remove(quest);
                QuestsComplete.Add(quest);
                QuestAgent qa = QuestAgents.Find(x => x.MQuest == quest);
                if (qa)
                {
                    QuestAgents.Remove(qa);
                    Destroy(qa.transform.parent.gameObject);
                }
                if (quest.isFirstAcceted)
                {
                    Sound.SoundManager.I.PlaySfx("SucessfulClick", Sound.SoundManager.ReadAudioClipType.ResourcesLoad);
                }
                foreach (Objective o in quest.Objectives)
                {
                    o.OnStateChangeEvent -= OnObjectiveStateChange;
                    if (o is CollectObjective co)
                    {
                        BagPanel.Instance.OnGetItemEvent -= co.UpdateCollectAmountUp;
                        BagPanel.Instance.OnLoseItemEvent -= co.UpdateCollectAmountDown;
                        if (!SaveManager.I.IsLoading && co.LoseItemAtSubmit) BagPanel.Instance.DropItem(int.Parse(co.ItemID), o.Amount);
                    }
                    if (o is KillObjective ko)
                    {
                        foreach (EnemyStatus enermy in GameManager.Instance.AllEnermy[ko.EnemyID])
                        {
                            enermy.OnDeathEvent -= ko.UpdateKillAmount;
                        }
                    }
                    if (o is TalkObjective to)
                    {
                        var talker = GameManager.Instance.AllQuestGiver[to.TalkerID];
                        talker.talkToThisObjectives.RemoveAll(x => x.talkObjective == to);
                        o.OnStateChangeEvent -= talker.TryRemoveObjective;
                    }
                    if (o is MoveObjective mo)
                    {
                        GameManager.Instance.AllQuestPoint[mo.PointID].OnMoveIntoEvent -= mo.UpdateMoveIntoStatus;
                        GameManager.Instance.AllQuestPoint[mo.PointID].OnMoveAwayEvent -= mo.UpdateMoveAwayStatus;
                    }
                    RemoveObjectiveMapIcon(o);
                }
                //获得奖励
                if (!SaveManager.I.IsLoading)
                {
                    foreach (ItemReward item in quest.MQuestReward.ItemRewards)
                    {
                        BagPanel.Instance.GetItem(item.ItemID, item.RewardNum);
                    }
                    //TODO 经验和金钱的处理
                    UserHeroVO newUserHeroVO = GameController.I.crtHero;
                    newUserHeroVO.money += quest.MQuestReward.Money;
                    newUserHeroVO.currentExp += quest.MQuestReward.EXP;
                    if (newUserHeroVO.currentExp >= newUserHeroVO.nextLvNeedExp)
                    {
                        newUserHeroVO.lv++;
                        newUserHeroVO.currentExp = newUserHeroVO.currentExp - newUserHeroVO.nextLvNeedExp;
                        SkillSystem.SkillManager.I.skillpoint++;
                    }
                    GameController.I.crtHero = newUserHeroVO;
                    StringBuilder @strBuilder = new StringBuilder(string.Format("奖励已获得：\n获得金币：{0},\n获得经验：{1},\n获得物品：", quest.MQuestReward.Money, quest.MQuestReward.EXP));
                    foreach (var item in quest.MQuestReward.ItemRewards)
                    {
                        @strBuilder.Append(ItemInfoManager.I.GetObjectInfoById(item.ItemID).name + "*" + item.RewardNum + "\n");
                    }
                    newUserHeroVO.tipMessage = @strBuilder.ToString();
                    PlayerManager.I.playerTrans.gameObject.SetActive(false);
                    gameCanvas.alpha = 0;
                    gameCanvas.blocksRaycasts = false;
                    Alert.Show("任务完成", newUserHeroVO.tipMessage, MainQuestAccept, quest,false);
                    OnQuestStatusChange?.Invoke();
                    return true;
                }
                CloseDescriptionWindow();
                OnQuestStatusChange?.Invoke();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 主任务的接收相关
        /// </summary>
        /// <param name="o"></param>
        public void MainQuestAccept(object o)
        {
            PlayerManager.I.playerTrans.gameObject.SetActive(true);
            if ((o as Quest).MOriginQuestGiver.ID == "NPC000")
                FindObjectOfType<SystemQuestGiver>().GiveQuest();
            gameCanvas.alpha = 1;
            gameCanvas.blocksRaycasts = true;
        }
        /// <summary>
        /// 放弃任务
        /// </summary>
        /// <param name="quest">要放弃的任务</param>
        public bool AbandonQuest(Quest quest)
        {
            //如果有任务并且任务是可以放弃的
            if (HasOngoingQuest(quest) && quest && quest.Abandonable)
            {
                //如果已经有任务需要该任务且正在进行
                if (HasQuestNeedAsCondition(quest, out var findQuest))
                {
                    Alert.Show("任务无法放弃", "由于任务[" + findQuest.Title + "]正在进行，无法放弃该任务。");
                    return false;
                }
                else
                {
                    quest.IsOngoing = false;
                    quest.isFirstAcceted = true;
                    QuestsOngoing.Remove(quest);
                    //相关目标状态初始化
                    foreach (Objective o in quest.Objectives)
                    {
                        o.OnStateChangeEvent -= OnObjectiveStateChange;
                        if (o is CollectObjective co)
                        {
                            co.CurrentAmount = 0;
                            co.amountWhenStart = 0;
                            BagPanel.Instance.OnGetItemEvent -= co.UpdateCollectAmountUp;
                            BagPanel.Instance.OnLoseItemEvent -= co.UpdateCollectAmountDown;
                        }
                        if (o is KillObjective ko)
                        {
                            ko.CurrentAmount = 0;
                            foreach (EnemyStatus enermy in GameManager.Instance.AllEnermy[ko.EnemyID])
                            {
                                enermy.OnDeathEvent -= ko.UpdateKillAmount;
                            }
                        }
                        if (o is TalkObjective to)
                        {
                            to.CurrentAmount = 0;
                            GameManager.Instance.AllQuestGiver[to.TalkerID].talkToThisObjectives.RemoveAll(x => x.talkObjective == to);
                        }
                        if (o is MoveObjective mo)
                        {
                            mo.CurrentAmount = 0;
                            GameManager.Instance.AllQuestPoint[mo.PointID].OnMoveIntoEvent -= mo.UpdateMoveIntoStatus;
                            GameManager.Instance.AllQuestPoint[mo.PointID].OnMoveAwayEvent -= mo.UpdateMoveAwayStatus;
                        }
                        RemoveObjectiveMapIcon(o);
                    }
                    //将任务交回
                    if (!quest.CmpltOnOriginalNPC)
                    {
                        quest.MOriginQuestGiver.TransferQuestToThis(quest);
                    }
                    OnQuestStatusChange?.Invoke();
                    ////收集当前的任务图标
                    //GameObject currentQuestMask = quest.MCurrentQuestGiver.transform.GetChild(0).gameObject;
                    //GameObjectPool.I.CollectObject(currentQuestMask);
                    return true;
                }
            }
            else if (!quest.Abandonable)
            {
                  Alert.Show("任务放弃失败","该任务无法放弃。");
            }
            return false;
        }
        /// <summary>
        /// 放弃当前展示的任务
        /// </summary>
        public void AbandonSelectedQuest()
        {
            //如果没有选择任务
            if (!SelectedQuest) return;
            Alert.Show("放弃任务", "已消耗的道具不会退回，确定放弃此任务吗？", o=>
            {
                //如果已经可以放弃任务
                if (AbandonQuest(SelectedQuest))
                {
                    //从当前的任务集合中将选择的任务移除
                    QuestAgent qa = QuestAgents.Find(x => x.MQuest == SelectedQuest);
                    if (qa)
                    {
                        QuestAgents.Remove(qa);
                        Transform qaParent = qa.transform.parent;
                        Destroy(qa.gameObject);
                        Destroy(qaParent.gameObject);
                    }
                    CloseDescriptionWindow();
                }
            });
            
        }
        /// <summary>
        /// 更新某个收集类任务目标，用于在其他前置目标完成时，更新后置收集类目标
        /// </summary>
        /// <param name="nextObj">下一个目标</param>
        void UpdateCollectObjectives(Objective objective)
        {
            if (objective != null || objective.NextObjective == null) return;
            Objective nextObjective = objective.NextObjective;
            while (nextObjective != null)
            {
                //若相邻后置目标不是收集类目标，该后置目标按顺序执行，其相邻后置也按顺序执行，且两者不可同时执行，则说明无法继续更新后置的收集类目标
                if (!(nextObjective is CollectObjective) && nextObjective.InOrder && nextObjective.NextObjective != null
                    && nextObjective.NextObjective.InOrder && nextObjective.OrderIndex < nextObjective.NextObjective.OrderIndex)
                {

                    return;
                }
                //若后置目标是收集目标可并行
                if (nextObjective is CollectObjective co)
                {
                    if (co.CheckBagAtAccept) co.CurrentAmount = BagPanel.Instance.GetItemAmountByID(int.Parse(co.ItemID));
                }
                nextObjective = nextObjective.NextObjective;
            }
        }
        /// <summary>
        /// 更新任务目标
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="befCmplt"></param>
        private void OnObjectiveStateChange(Objective objective, bool befCmplt)
        {
            Debug.Log(objective.DisplayName + objective.IsComplete);
            if (!befCmplt && objective.IsComplete)
            {
                UpdateCollectObjectives(objective);
                //Debug.Log("\"" + objective.DisplayName + "\"" + "从没完成变成完成");
                Objective nextToDo = null;
                Quest quest = QuestsOngoing.Find(q => q.Objectives.Contains(objective));
                List<Objective> parallelObj = new List<Objective>();
                if (quest != null)
                {
                    for (int i = 0; i < quest.Objectives.Count - 1; i++)
                    {
                        if (quest.Objectives[i] == objective)
                        {
                            for (int j = i - 1; j > -1; j--)//往前找可以并行的目标
                            {
                                Objective prevObj = quest.Objectives[j];
                                if (!prevObj.Parallel) break;//只要碰到一个不能并行的，就中断
                                else parallelObj.Add(prevObj);
                            }
                            for (int j = i + 1; j < quest.Objectives.Count; j++)//往后找可以并行的目标
                            {
                                Objective nextObj = quest.Objectives[j];
                                if (!nextObj.Parallel)//只要碰到一个不能并行的，就中断
                                {
                                    if (nextObj.AllPrevObjCmplt && !nextObj.IsComplete)
                                        nextToDo = nextObj;//同时，若该不能并行目标的所有前置目标都完成了，那么它就是下一个要做的目标
                                    break;
                                }
                                else parallelObj.Add(nextObj);
                            }
                            break;
                        }
                    }
                }
                if (nextToDo != null)//当目标不能并行时此变量才不为空，所以此时表示所有后置目标都是可并行的，或者不存在后置目标
                {
                    parallelObj.RemoveAll(x => x.IsComplete);//把所有已完成的可并行目标去掉
                                                             /*if (parallelObj.Count > 0)//剩下未完成的可并行目标，则随机选一个作为下一个要做的目标
                                                                 nextToDo = parallelObj[Random.Range(0, parallelObj.Count)];*/
                    foreach (var obj in parallelObj)
                        CreateObjectiveMapIcon(obj);
                }
                else CreateObjectiveMapIcon(nextToDo);
                RemoveObjectiveMapIcon(objective);
                OnQuestStatusChange?.Invoke();
            }
        }

       
        /// <summary>
        /// 判断是否有该任务
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public bool HasOngoingQuest(Quest quest)
        {
            return QuestsOngoing.Contains(quest);
        }
        /// <summary>
        /// 判断是否完成该任务
        /// </summary>
        /// <param name="quest"></param>
        /// <returns></returns>
        public bool HasCompleteQuest(Quest quest)
        {
            return QuestsComplete.Contains(quest);
        }
        /// <summary>
        /// 根据id判断是否完成该任务
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public bool HasCompleteQuestWithID(string questID)
        {
            return QuestsComplete.Exists(x => x._ID == questID);
        }
        /// <summary>
        /// 判定是否有某个任务需要某数量的某个道具
        /// </summary>
        /// <param name="item">要判定的道具ID</param>
        /// <param name="leftAmount">要判定的数量</param>
        /// <returns>是否需要该道具</returns>
        public bool HasQuestRequiredItem(string itemID, int leftAmount)
        {
            return QuestsRequireItem(itemID, leftAmount).Count() > 0;
        }
        private IEnumerable<Quest> QuestsRequireItem(string itemID, int leftAmount)
        {
            return questsOngoing.FindAll(x => BagPanel.Instance.IsQuestRequiredItem(x, itemID, leftAmount)).AsEnumerable();
        }
        /// <summary>
        /// 是否有任务需要该条件
        /// </summary>
        /// <param name="quest"></param>
        /// <param name="findQuest"></param>
        /// <returns></returns>
        public bool HasQuestNeedAsCondition(Quest quest, out Quest findQuest)
        {
            findQuest = questsOngoing.Find(x => x.AcceptConditions.Exists(y => y.ConditionType== ConditionType.CompleteQuest && y.CompleteQuest._ID == quest._ID));
            return findQuest != null;
        }
        #endregion

        #region UI相关
        /// <summary>
        /// 显示描述信息
        /// </summary>
        /// <param name="quest"></param>
        public void ShowDescription(Quest quest)
        {
            if (!quest) return;
            QuestAgent qa = QuestAgents.Find(x => x.MQuest == quest);
            if (qa)
            {
                //如果有选择任务但任务不是本任务
                if (SelectedQuest && SelectedQuest != quest)
                {
                    QuestAgent tqa = QuestAgents.Find(x => x.MQuest == SelectedQuest);
                    tqa.TitleText.color = Color.black;
                }
                qa.TitleText.color = Color.blue;
            }
            //将选择的任务选择为本任务
            SelectedQuest = quest;
            UpdateObjectivesText();
            moneyText.text = string.Format("<size=20>金币:{0}</size>", quest.MQuestReward.EXP);
            expText.text = string.Format("<size=20>经验:{0}</size>", quest.MQuestReward.Money);
            //先将物品奖励置空
            foreach (BagItemGrid rwg in rewardGrids)
            {
                BagItem bagItem = rwg.transform.FindChildComponentByName<BagItem>("Reward");
                bagItem.id = 0;
                bagItem.num = 0;
                bagItem.gameObject.SetActive(false);
            }
            //再更新
            foreach (ItemReward item in quest.MQuestReward.ItemRewards)
                foreach (BagItemGrid rw in rewardGrids)
                {
                    BagItem bagItem = rw.transform.FindChildComponentByName<BagItem>("Reward");
                    if (bagItem.id == 0 && bagItem.num == 0)
                    {
                        bagItem.gameObject.SetActive(true);
                        bagItem.id = item.ItemID;
                        bagItem.num = item.RewardNum;
                        string spriteName = ItemInfoManager.I.GetObjectInfoById(item.ItemID).icon_name;
                        Texture2D iconTexture = ResourceManager.Load<Texture2D>(spriteName);
                        bagItem.transform.FindChildComponentByName<Image>("ItemImage").overrideSprite =
                            Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
                        bagItem.transform.FindChildComponentByName<Text>("Number").text = item.RewardNum.ToString();
                        break;
                    }
                }
            ItemPanel.I.isBagShow = false;
            ItemPanel.I.openType = OpenWindowType.QuestDescription;
            abandonButton.gameObject.SetActive(quest.Abandonable);
        }
        /// <summary>
        /// 更新目标文本信息
        /// </summary>
        public void UpdateObjectivesText()
        {
            foreach (QuestAgent qa in QuestAgents)
                qa.UpdateQuestStatus();
            if (SelectedQuest == null) return;
            string objectives = string.Empty;
            //更新选择的任务的每一个目标
            for (int i = 0; i < SelectedQuest.Objectives.Count; i++)
                objectives += SelectedQuest.Objectives[i].DisplayName +
                    "[" + SelectedQuest.Objectives[i].CurrentAmount + "/" + SelectedQuest.Objectives[i].Amount + "]" +
                    (SelectedQuest.Objectives[i].IsComplete ? "(达成)\n" : "\n");
            //更新任务描述信息
            descriptionText.text = string.Format("<size=25><b>{0}</b></size>\n<size=20>[委托人: {1}]\n{2}</size>\n\n<size=25><b>任务目标{3}</b></size>\n{4}",
                SelectedQuest.Title,
                SelectedQuest.MOriginQuestGiver.Name,
                SelectedQuest.Description,
                SelectedQuest.IsComplete ? "(完成)" : SelectedQuest.IsOngoing ? "(进行中)" : string.Empty,
                objectives);
        }
        /// <summary>
        /// 产生任务图标
        /// </summary>
        /// <param name="objective"></param>
        private void CreateObjectiveMapIcon(Objective objective)
        {
            if (objective == null) return;
            Vector3 destination;
            if (objective is TalkObjective to)
            {
                if (GameManager.I.AllQuestGiver.TryGetValue(to.TalkerID, out QuestGiver talkerFound))
                {
                    destination = talkerFound.currentPosition;
                    CreateIcon();
                }
            }
            else if (objective is KillObjective ko)
            {
                if (GameManager.I.AllEnermy.TryGetValue(ko.EnemyID, out List<EnemyStatus> enemiesFound) && enemiesFound.Count > 0)
                {
                    EnemyStatus enemy = enemiesFound.FirstOrDefault();
                    if (enemy)
                    {
                        destination = enemy.transform.position;
                        CreateIcon();
                    }
                }
            }
            else if (objective is MoveObjective mo)
            {
                if (GameManager.I.AllQuestPoint.TryGetValue(mo.PointID, out var pointsFound))
                {
                    destination = pointsFound.transform.position;
                    CreateIcon();
                }
            }
            void CreateIcon()
            {
                var icon = (objective is KillObjective ?
                    MapManager.Instance.CreateMapIcon(questIcon, new Vector2(48, 48), destination, true, 144f, MapIconType.Objective, false, objective.DisplayName) :
                    MapManager.Instance.CreateMapIcon(questIcon, new Vector2(48, 48), destination, true, MapIconType.Objective, false, objective.DisplayName));
                if (icon)
                {
                    if (questIcons.TryGetValue(objective, out MapIcon iconExist))
                    {
                        MapManager.Instance.RemoveMapIcon(iconExist, true);
                        questIcons[objective] = icon;
                    }
                    else questIcons.Add(objective, icon);
                }
            }
        }
        /// <summary>
        /// 移除任务图标
        /// </summary>
        /// <param name="objective"></param>
        private void RemoveObjectiveMapIcon(Objective objective)
        {
            if (objective == null) return;
            if (questIcons.TryGetValue(objective, out MapIcon icon))
            {
                questIcons.Remove(objective);
                MapManager.Instance.RemoveMapIcon(icon, true);
            }
        }
        /// <summary>
        /// 关闭任务描述窗口
        /// </summary>
        public void CloseDescriptionWindow()
        {
            QuestAgent qa = QuestAgents.Find(x => x.MQuest == SelectedQuest);
            if (qa) qa.TitleText.color = Color.black;
            SelectedQuest = null;
            descriptionWindow.alpha = 0;
            descriptionWindow.blocksRaycasts = false;
        }
        /// <summary>
        /// 打开任务描述窗口
        /// </summary>
        /// <param name="questAgent"></param>
        public void OpenDescriptionWindow(QuestAgent questAgent)
        {
            DialogueManager.Instance.CloseQuestDescriptionWindow();
            ShowDescription(questAgent.MQuest);
            descriptionWindow.alpha = 1;
            descriptionWindow.blocksRaycasts = true;
        }
        /// <summary>
        /// 关闭任务窗口
        /// </summary>
        public void CloseQuestWindow()
        {
            questsWindow.alpha = 0;
            questsWindow.blocksRaycasts = false;
            CloseDescriptionWindow();
        }
        /// <summary>
        /// 打开任务窗口
        /// </summary>
        public void OpenQuestWindow()
        {
            questsWindow.alpha = 1;
            questsWindow.blocksRaycasts = true;
            DialogueManager.Instance.CloseQuestDescriptionWindow();
        }
        #endregion
    }
}
