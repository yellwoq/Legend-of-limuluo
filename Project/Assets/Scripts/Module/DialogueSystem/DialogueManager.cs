using Bag;
using Common;
using QuestSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable CS0649

namespace DialogueSystem
{
    /// <summary>
    /// 处理对话的单例，它将用于处理对话的成句进行、处理对话选项、处理对话型目标等
    /// </summary>
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        /// <summary>
        /// 对话窗口
        /// </summary>
        [SerializeField]
        private CanvasGroup dialogueWindow;
        /// <summary>
        /// 对话者头像
        /// </summary>
        [SerializeField]
        private Image talkerImage;
        /// <summary>
        /// 用于显示对话者姓名的文本UI
        /// </summary>
        [SerializeField]
        private Text nameText;
        /// <summary>
        /// 用于显示对话者的对话内容的UI
        /// </summary>
        [SerializeField]
        private Text wordsText;
        /// <summary>
        /// 选项父物体，显示选项（任务或者继续）
        /// </summary>
        [SerializeField]
        private Transform optionsParent;
        /// <summary>
        /// 选项预制体，用于生成选项
        /// </summary>
        [SerializeField]
        private GameObject optionPrefab;
        /// <summary>
        /// 对话类型，默认为正常对话
        /// </summary>
        [SerializeField, ReadOnly]
        private DialogueType dialogueType = DialogueType.Normal;
        /// <summary>
        /// 所有说的话
        /// </summary>
        private Queue<DialogueWords> Words = new Queue<DialogueWords>();
        /// <summary>
        /// 任务给与者NPC
        /// </summary>
        private QuestGiver questGiver;
        /// <summary>
        /// 对话者
        /// </summary>
        private Talker talker;

        /// <summary>
        /// 查看上一页按钮
        /// </summary>
        [Space, Header("选项相关")]
        [SerializeField]
        private Button pageUpButton;
        /// <summary>
        /// 查看下一页按钮
        /// </summary>
        [SerializeField]
        private Button pageDownButton;
        /// <summary>
        /// 关闭对话窗口按钮
        /// </summary>
        [SerializeField]
        private Button closeWindownButton;
        /// <summary>
        /// 取消对话按钮
        /// </summary>
        [SerializeField]
        private Button cancelDialogueButton;
        [SerializeField]
        private Text pageText;
        /// <summary>
        /// 文本高度
        /// </summary>
        [SerializeField]
        private float textLineHeight = 22.35832f;
        /// <summary>
        /// 行的数量
        /// </summary>
        [SerializeField]
        private int lineAmount = 5;

        private int page = 1;
        /// <summary>
        /// 页数
        /// </summary>
        public int Page
        {
            get
            {
                return page;
            }
            set
            {
                if (value > 1) page = value;
                else page = 1;
            }
        }
        /// <summary>
        /// 最大页数
        /// </summary>
        private int MaxPage = 1;
        /// <summary>
        /// 选项
        /// </summary>
        [HideInInspector]
        public List<OptionAgent> OptionAgents;
        /// <summary>
        /// 谈话目标
        /// </summary>
        private TalkObjective talkObjective;
        /// <summary>
        /// 当前处理的任务
        /// </summary>
        private Quest MQuest;
        /// <summary>
        /// 任务按钮
        /// </summary>
        [Space, Header("任务详情UI相关")]
        [SerializeField]
        private Button questButton;
        /// <summary>
        /// 任务描述窗口
        /// </summary>
        [SerializeField]
        private CanvasGroup descriptionWindow;

        public CanvasGroup DescriptionWindow
        {
            get
            {
                return descriptionWindow;
            }
        }
        /// <summary>
        /// 任务描述文本
        /// </summary>
        [SerializeField]
        private Text descriptionText;
        /// <summary>
        /// 经验文本
        /// </summary>
        [SerializeField]
        private Text EXPText;
        /// <summary>
        /// 金钱文本
        /// </summary>
        [SerializeField]
        private Text MoneyText;
        /// <summary>
        /// 所得到的物品奖励
        /// </summary>
        [SerializeField]
        private BagItemGrid[] rewardGrids;

        protected override void Initialize()
        {
            pageUpButton.onClick.AddListener(OptionPageUp);
            pageDownButton.onClick.AddListener(OptionPageDown);
            closeWindownButton.onClick.AddListener(CloseDialogueWindow);
            cancelDialogueButton.onClick.AddListener(GotoDefault);
            questButton.onClick.AddListener(LoadTalkerQuest);
        }
        #region 开始新对话
        /// <summary>
        /// 开始对话
        /// </summary>
        /// <param name="dialogue">所要开始的对话</param>
        public void StartDialogue(Dialogue dialogue)
        {
            //如果对话不存在或没有对话内容
            if (dialogue.Words.Count < 1 || !dialogue) return;
            //清空储存话语内容的数据
            Words.Clear();
            //话语入队
            foreach (DialogueWords saying in dialogue.Words)
            {
                Words.Enqueue(saying);
            }
            //开始对话
            SayNextWords();
            //如果没有选项
            if (OptionAgents.Count < 1) optionsParent.gameObject.SetActive(false);
            wordsText.gameObject.SetActive(true);
            OpenDialogueWindow();
            pageUpButton.gameObject.SetActive(false);
            pageDownButton.gameObject.SetActive(false);
            pageText.gameObject.SetActive(false);
        }
        /// <summary>
        /// 开始任务给与者对话
        /// </summary>
        /// <param name="questGiver"></param>
        public void StartQuestGiverDialogue(QuestGiver questGiver)
        {
            this.questGiver = questGiver;
            questGiver.OnTalkBegin();
            dialogueType = DialogueType.Giver;
            if (questGiver.QuestInstances.Count > 0) questButton.gameObject.SetActive(true);
            else questButton.gameObject.SetActive(false);
            StartDialogue(questGiver.DefaultDialogue);
        }
        /// <summary>
        /// 开始正常对话
        /// </summary>
        /// <param name="talker"></param>
        public void StartNormalTalkerDialogue(Talker talker)
        {
            this.talker = talker;
            talker.OnTalkBegin();
            dialogueType = DialogueType.Normal;
            questButton.gameObject.SetActive(false);
            StartDialogue(talker.DefaultDialogue);
        }
        /// <summary>
        /// 开始任务对话
        /// </summary>
        /// <param name="quest"></param>
        public void StartQuestDialogue(Quest quest)
        {
            MQuest = quest;
            dialogueType = DialogueType.Quest;
            ClearOptions();
            //如果任务还没有被接取
            if (!MQuest.IsComplete && !MQuest.IsOngoing) StartDialogue(quest.BeginDialogue);
            //如果任务正在进行
            else if (!MQuest.IsComplete && MQuest.IsOngoing) StartDialogue(quest.OngoingDialogue);
            //如果任务已经完成
            else StartDialogue(quest.CompleteDialogue);
        }
        /// <summary>
        /// 开始目标对话
        /// </summary>
        /// <param name="talkObjective"></param>
        public void StartObjectiveDialogue(TalkObjective talkObjective)
        {
            this.talkObjective = talkObjective;
            dialogueType = DialogueType.Objective;
            ClearOptions();
            StartDialogue(talkObjective.Dialogue);
        }
        #endregion

        #region 处理对话选项
        /// <summary>
        /// 生成继续按钮选项
        /// </summary>
        void MakeContinueOption()
        {
            //从选项中查找选项类型是继续的按钮
            OptionAgent oa = OptionAgents.Find(x => x.optionType == OptionType.Continue);

            if (oa) OpenOptionArea();
            if (Words.Count > 1)
            {
                //如果还有话没说完，弹出一个“继续”按钮
                if (!oa)
                {
                    oa = Instantiate(optionPrefab, optionsParent).GetComponent<OptionAgent>();
                    oa.optionType = OptionType.Continue;
                    oa.TitleText.text = "继续";
                    OptionAgents.Add(oa);
                    OpenOptionArea();
                }
            }
            else if (oa)
            {
                //如果话说完了，把“继续”按钮去掉
                OptionAgents.Remove(oa);
                Destroy(oa.gameObject);
            }
        }
        /// <summary>
        /// 生成任务列表的选项
        /// </summary>
        void MakeTalkerQuestOption()
        {
            int leftLineCount = lineAmount - (int)(wordsText.preferredHeight / textLineHeight);
            int index = 1;
            ClearOptions();
            foreach (Quest quest in questGiver.QuestInstances)
            {
                //如果还没有完成该任务并且可以接受
                if (!QuestManager.Instance.HasCompleteQuest(quest) && quest.AcceptAble)
                {
                    OptionAgent oa = Instantiate(optionPrefab, optionsParent).GetComponent<OptionAgent>();
                    oa.optionType = OptionType.Quest;
                    oa.MQuest = quest;
                    oa.TitleText.text = quest.Title + (quest.IsComplete ? "(完成)" : quest.IsOngoing ? "(进行中)" : string.Empty);
                    OptionAgents.Add(oa);
                    if (index > leftLineCount) oa.gameObject.SetActive(false);
                    index++;
                }
            }
            MaxPage = Mathf.CeilToInt(OptionAgents.Count * 1.0f / (leftLineCount * 1.0f));//总页数
            if (MaxPage > 1)
            {
                pageUpButton.gameObject.SetActive(false);
                pageDownButton.gameObject.SetActive(true);
                pageText.gameObject.SetActive(true);
                pageText.text = Page.ToString() + "/" + MaxPage.ToString();
            }
            else
            {
                pageUpButton.gameObject.SetActive(false);
                pageDownButton.gameObject.SetActive(false);
                pageText.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 生成对话目标列表的选项
        /// </summary>
        void MakeTalkerObjectiveOption()
        {
            //剩余行数
            int leftLineCount = lineAmount - (int)(wordsText.preferredHeight / textLineHeight);
            int index = 1;
            //如果任务给与者的对话目标不为空
            if (questGiver.talkToThisObjectives != null && questGiver.talkToThisObjectives.Count > 0)
            {
                ClearOptions();
                //遍历任务给与者的所有的对话目标
                foreach (TalkObjectiveInfo toi in questGiver.talkToThisObjectives)
                {
                    //如果前置目标都已经完成并且没有后置目标在进行
                    if (toi.talkObjective.AllPrevObjCmplt && !toi.talkObjective.HasNextObjOngoing)
                    {
                        OptionAgent oa = Instantiate(optionPrefab, optionsParent).GetComponent<OptionAgent>();
                        oa.optionType = OptionType.TalkObjective;
                        oa.TitleText.text = toi.questTitle;
                        oa.talkObjective = toi.talkObjective;
                        OptionAgents.Add(oa);
                        if (index > leftLineCount) oa.gameObject.SetActive(false);
                        index++;
                    }
                }
            }
            //计算最大页数
            MaxPage = Mathf.CeilToInt(OptionAgents.Count * 1.0f / (leftLineCount * 1.0f));
            if (MaxPage > 1)
            {
                pageUpButton.gameObject.SetActive(false);
                pageDownButton.gameObject.SetActive(true);
                pageText.gameObject.SetActive(true);
                pageText.text = Page.ToString() + "/" + MaxPage.ToString();
            }
            else
            {
                pageUpButton.gameObject.SetActive(false);
                pageDownButton.gameObject.SetActive(false);
                pageText.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 向前翻页
        /// </summary>
        public void OptionPageUp()
        {
            int leftLineCount = lineAmount - (int)(wordsText.preferredHeight / textLineHeight);
            if (page > 0)
            {
                Page--;
                for (int i = 0; i < leftLineCount; i++)
                {

                    if ((page - 1) * leftLineCount + i < OptionAgents.Count && (page - 1) * leftLineCount + i >= 0)
                        OptionAgents[(page - 1) * leftLineCount + i].gameObject.SetActive(true);
                    if (page * leftLineCount + i >= 0 && page * leftLineCount + i < OptionAgents.Count)
                        OptionAgents[page * leftLineCount + i].gameObject.SetActive(false);
                }
            }
            if (Page == 1 && MaxPage > 1)
            {
                pageUpButton.gameObject.SetActive(false);
                pageDownButton.gameObject.SetActive(true);
            }
            else
            {
                pageUpButton.gameObject.SetActive(true);
                pageDownButton.gameObject.SetActive(true);
            }
            pageText.text = Page.ToString() + "/" + MaxPage.ToString();
        }
        /// <summary>
        /// 向后翻页
        /// </summary>
        public void OptionPageDown()
        {
            int leftLineCount = lineAmount - (int)(wordsText.preferredHeight / textLineHeight);
            if (page < Mathf.CeilToInt(OptionAgents.Count * 1.0f / (leftLineCount * 1.0f)))
            {
                for (int i = 0; i < leftLineCount; i++)
                {
                    if ((page - 1) * leftLineCount + i < OptionAgents.Count && (page - 1) * leftLineCount + i >= 0)
                        OptionAgents[(page - 1) * leftLineCount + i].gameObject.SetActive(false);
                    if (page * leftLineCount + i >= 0 && page * leftLineCount + i < OptionAgents.Count)
                        OptionAgents[page * leftLineCount + i].gameObject.SetActive(true);
                }
                Page++;
            }
            if (Page == MaxPage && MaxPage > 1)
            {
                pageUpButton.gameObject.SetActive(true);
                pageDownButton.gameObject.SetActive(false);
            }
            else
            {
                pageUpButton.gameObject.SetActive(true);
                pageDownButton.gameObject.SetActive(true);
            }
            pageText.text = Page.ToString() + "/" + MaxPage.ToString();
        }
        #endregion

        #region 处理每句话
        /// <summary>
        /// 转到下一句话
        /// </summary>
        public void SayNextWords()
        {
            MakeContinueOption();
            //因为Dequeue之后，话就没了，Words.Count就不是1了，而是0，所以要在此之前做这一步
            if (Words.Count == 1)
            {
                HandlingLastWords();

            }
            if (Words.Count > 0)
            {
                nameText.text = Words.Peek().TalkerName;
                talkerImage.sprite = Words.Peek().TalkerInfo.HeadIcon;
                wordsText.text = Words.Dequeue().Words;
            }
            if (dialogueType == DialogueType.Normal)
                CloseDialogueWindow();
        }

        /// <summary>
        /// 处理最后一句对话
        /// </summary>
        private void HandlingLastWords()
        {
            //如果对话类型是任务给与者对话目标
            if (dialogueType == DialogueType.Giver && questGiver)
            {
                questGiver.OnTalkFinished();
                MakeTalkerObjectiveOption();
            }
            //如果对话类型是普通对话
            else if (dialogueType == DialogueType.Normal && talker != null)
            {
                talker.OnTalkFinished();
            }
            //如果对话类型是目标对话
            else if (dialogueType == DialogueType.Objective && talkObjective != null)
            {
                HandlingLastObjectiveWords();
            }
            //如果对话类型是任务
            else if (dialogueType == DialogueType.Quest && MQuest)
            {
                HandlingLastQuestWords();
            }
            //如果该目标身上有对话类目标，而且每个目标都有正确对话数据，则展示对话选项
            if (OptionAgents.FindAll(x => x.optionType == OptionType.TalkObjective).Count > 0 && !questGiver.talkToThisObjectives.Exists(x => x.talkObjective.Dialogue == null))
            {
                optionsParent.gameObject.SetActive(true);
            }
        }
        /// <summary>
        /// 处理最后一句目标的对话
        /// </summary>
        private void HandlingLastObjectiveWords()
        {
            talkObjective.UpdateTalkStatus();
            //如果目标已经完成
            if (talkObjective.IsComplete)
            {
                OptionAgent oa = OptionAgents.Find(x => x.talkObjective == talkObjective);
                if (oa && oa.gameObject)
                {
                    //去掉该对话目标自身的对话型目标选项
                    OptionAgents.Remove(oa);
                    Destroy(oa.gameObject);
                }
                //目标已经完成，不再需要保留在对话人的目标列表里，从对话人的对话型目标里删掉相应信息
                questGiver.talkToThisObjectives.RemoveAll(x => x.talkObjective == talkObjective);
            }
            talkObjective = null;//重置管理器的对话目标以防出错
            QuestManager.Instance.UpdateObjectivesText();
        }
        /// <summary>
        /// 处理最后一句任务的对话
        /// </summary>
        private void HandlingLastQuestWords()
        {
            if (!MQuest.IsOngoing || MQuest.IsComplete)
            {
                ClearOptions();
                //若是任务对话的最后一句，则根据任务情况弹出确认按钮
                OptionAgent yes = Instantiate(optionPrefab, optionsParent).GetComponent<OptionAgent>();
                OptionAgents.Add(yes);
                yes.optionType = OptionType.Comfirm;
                yes.MQuest = MQuest;
                yes.YesOrNo = true;
                yes.TitleText.text = MQuest.IsComplete ? "完成" : "接受";
                //如果任务未完成
                if (!MQuest.IsComplete)
                {
                    OptionAgent no = Instantiate(optionPrefab, optionsParent).GetComponent<OptionAgent>();
                    OptionAgents.Add(no);
                    no.optionType = OptionType.Comfirm;
                    no.YesOrNo = false;
                    no.TitleText.text = "拒绝";
                }
                OpenQuestDescriptionWindow(MQuest);
            }
            MQuest = null;
        }
        #endregion

        #region UI相关
        /// <summary>
        /// 打开对话窗口
        /// </summary>
        public void OpenDialogueWindow()
        {
            dialogueWindow.alpha = 1;
            dialogueWindow.blocksRaycasts = true;
            Time.timeScale = 0;
            CanvasGroup gameGroup=GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<CanvasGroup>();
            gameGroup.alpha = 0;
            gameGroup.blocksRaycasts = false;
        }
        /// <summary>
        /// 关闭对话窗口
        /// </summary>
        public void CloseDialogueWindow()
        {
            dialogueWindow.alpha = 0;
            dialogueWindow.blocksRaycasts = false;
            Time.timeScale = 1;
            questGiver = null;
            MQuest = null;
            ClearOptions();
            CloseQuestDescriptionWindow();
            CanvasGroup gameGroup = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<CanvasGroup>();
            gameGroup.alpha = 1;
            gameGroup.blocksRaycasts = true;
        }
        /// <summary>
        /// 打开选项区域
        /// </summary>
        public void OpenOptionArea()
        {
            if (OptionAgents.Count < 1) return;
            optionsParent.gameObject.SetActive(true);
            QuestManager.Instance.CloseDescriptionWindow();
        }
        //关闭选项区域
        public void CloseOptionArea()
        {
            optionsParent.gameObject.SetActive(false);
            CloseQuestDescriptionWindow();
        }
        /// <summary>
        /// 打开任务描述窗口
        /// </summary>
        /// <param name="quest"></param>
        public void OpenQuestDescriptionWindow(Quest quest)
        {
            QuestManager.Instance.CloseDescriptionWindow();
            ShowDescription(quest);
            descriptionWindow.alpha = 1;
            descriptionWindow.blocksRaycasts = true;
        }
        /// <summary>
        /// 关闭任务描述窗口
        /// </summary>
        public void CloseQuestDescriptionWindow()
        {
            MQuest = null;
            descriptionWindow.alpha = 0;
            descriptionWindow.blocksRaycasts = false;
        }
        /// <summary>
        /// 显示任务描述信息
        /// </summary>
        /// <param name="quest"></param>
        private void ShowDescription(Quest quest)
        {
            if (quest == null) return;
            MQuest = quest;
            descriptionText.text = string.Format("<size=16><b>{0}</b></size>\n[委托人: {1}]\n{2}", MQuest.Title, MQuest.MOriginQuestGiver.Name, MQuest.Description);
            EXPText.text = string.Format("<size=14>经验:{0}</size>", quest.MQuestReward.EXP);
            MoneyText.text = string.Format("<size=14>金币:{0}</size>", quest.MQuestReward.Money);

            foreach (BagItemGrid rwg in rewardGrids)
            {
                BagItem bagItem = rwg.transform.FindChildComponentByName<BagItem>("Reward");
                bagItem.id = 0;
                bagItem.num = 0;
                bagItem.gameObject.SetActive(false);
            }
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
            ItemPanel.I.openType = OpenWindowType.DialogueDescription;
        }
        #endregion

        #region 其它
        /// <summary>
        /// 读取对话者任务
        /// </summary>
        public void LoadTalkerQuest()
        {
            if (questGiver == null) return;
            questButton.gameObject.SetActive(false);
            Skip();
            MakeTalkerQuestOption();
            OpenOptionArea();
        }
        /// <summary>
        /// 跳过
        /// </summary>
        public void Skip()
        {
            while (Words.Count > 0)
                SayNextWords();
        }
        /// <summary>
        /// 切换默认
        /// </summary>
        public void GotoDefault()
        {
            ClearOptions();
            CloseQuestDescriptionWindow();
            StartQuestGiverDialogue(questGiver);
        }
        /// <summary>
        /// 清空选项，恢复默认
        /// </summary>
        private void ClearOptions()
        {
            for (int i = 0; i < OptionAgents.Count; i++)
            {
                if (OptionAgents[i]) Destroy(OptionAgents[i].gameObject);
            }
            OptionAgents.Clear();
            Page = 1;
            pageUpButton.gameObject.SetActive(false);
            pageDownButton.gameObject.SetActive(false);
            pageText.gameObject.SetActive(false);
        }
        /// <summary>
        /// 清空选项除了继续
        /// </summary>
        private void ClearOptionExceptContinue()
        {
            for (int i = 0; i < OptionAgents.Count; i++)
            {
                if (OptionAgents[i] && OptionAgents[i].optionType != OptionType.Continue) Destroy(OptionAgents[i].gameObject);
            }
            OptionAgents.RemoveAll(x => !x.gameObject);
            Page = 1;
            pageUpButton.gameObject.SetActive(false);
            pageDownButton.gameObject.SetActive(false);
            pageText.gameObject.SetActive(false);
        }
        #endregion

        private enum DialogueType
        {
            Normal,//普通对话
            Quest,//任务对话
            Giver,//对话型目标对话
            Objective//目标对话
        }
    }
}
