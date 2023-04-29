using DialogueSystem;
using MapSystem;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649
namespace QuestSystem
{
    /// <summary>
    /// 触发对话的NPC
    /// </summary>
    public class Talker : MonoBehaviour, ITalkAble
    {
        [SerializeField]
        protected TalkerInformation currentTalkerInfo;
        [SerializeField]
        protected string _ID;
        public string ID => _ID;
        [SerializeField]
        protected string _name;
        public string Name => _name;
        [SerializeField]
        protected Sprite heroIcon;
        public Sprite HeadIcon => heroIcon;
        [SerializeField]
        protected Dialogue defaultDialogue;
        public Dialogue DefaultDialogue => defaultDialogue;
        [SerializeField]
        protected CharacterType characterType;
        public CharacterType MCharacterType => characterType;
        [SerializeField]
        protected bool isVendor;
        public bool IsVendor => isVendor;
        [SerializeField]
        private MapIconHolder iconHolder;
        [SerializeField]
        public Vector3 currentPosition;
        /// <summary>
        /// 存储对象身上的对话型目标
        /// </summary>
        [SerializeField,NonReorderable]
        public List<TalkObjectiveInfo> talkToThisObjectives = new List<TalkObjectiveInfo>();

        public event NPCTalkListener OnTalkBeginEvent;
        public event NPCTalkListener OnTalkFinishedEvent;

        protected virtual void Awake()
        {
            InitInfo();
        }
        /// <summary>
        /// 初始化信息
        /// </summary>
        public void InitInfo()
        {
            if (currentTalkerInfo)
            {
                _ID = currentTalkerInfo.ID;
                _name = currentTalkerInfo.Name;
                heroIcon = currentTalkerInfo.HeadIcon;
                defaultDialogue = currentTalkerInfo.DefaultDialogue;
                characterType = currentTalkerInfo.ChType;
                isVendor = currentTalkerInfo.IsVendor;
            }
            else
                Debug.LogWarning("当前没有NPC信息");
            //实现在地图中点击显示名字的功能
            iconHolder = GetComponent<MapIconHolder>();
            if (iconHolder)
            {
                iconHolder.textToDisplay = GetMapIconName();
                iconHolder.iconEvents.RemoveAllListner();
                iconHolder.iconEvents.onFingerClick.AddListener(ShowNameAtMousePosition);
                iconHolder.iconEvents.onMouseEnter.AddListener(ShowNameAtMousePosition);
                iconHolder.iconEvents.onMouseExit.AddListener(HideNameImmediately);
            }
            currentPosition = transform.position;
        }
        
        /// <summary>
        /// 开始对话时调用
        /// </summary>
        public void OnTalkBegin()
        {
            OnTalkBeginEvent?.Invoke();
        }
        /// <summary>
        /// 结束对话时调用
        /// </summary>
        public void OnTalkFinished()
        {
            OnTalkFinishedEvent?.Invoke();
            QuestManager.Instance.UpdateObjectivesText();
        }
        /// <summary>
        /// 获取地图中显示的图标名字
        /// </summary>
        /// <returns></returns>
        private string GetMapIconName()
        {
            System.Text.StringBuilder name = new System.Text.StringBuilder(Name);
            //如果是商人或者是有任务的非系统管理者
            if (currentTalkerInfo.IsVendor || (currentTalkerInfo.QuestsStored.Count > 0 && ID != "NPC000"))
            {
                name.Append("(");
                if (currentTalkerInfo.IsVendor) name.Append("商人");
                else name.Append(currentTalkerInfo.ChType);
                name.Append(")");
            }
            return name.ToString();
        }
        /// <summary>
        /// 在鼠标点击的位置显示名字
        /// </summary>
        private void ShowNameAtMousePosition()
        {
            int time = -1;
#if UNITY_ANDROID
        time = 2;
#endif
            TipsManager.Instance.ShowText(Input.mousePosition, GetMapIconName(), time);
        }
        /// <summary>
        /// 迅速隐藏
        /// </summary>
        private void HideNameImmediately()
        {
            TipsManager.Instance.Hide();
        }
    }

    public delegate void NPCTalkListener();

    /// <summary>
    /// 可对话接口
    /// </summary>
    public interface ITalkAble
    {
        Dialogue DefaultDialogue { get; }
        event NPCTalkListener OnTalkBeginEvent;
        event NPCTalkListener OnTalkFinishedEvent;
        void OnTalkBegin();
        void OnTalkFinished();
    }

    /// <summary>
    /// 记录对话目标信息，用于对话系统生成对话目标选项
    /// </summary>
    public class TalkObjectiveInfo
    {
        /// <summary>
        /// 对话主题
        /// </summary>
        [SerializeField]
        public string questTitle;
        /// <summary>
        /// 对话目标
        /// </summary>
        [SerializeField]
        public TalkObjective talkObjective;

        public TalkObjectiveInfo(string title, TalkObjective objective)
        {
            questTitle = title;
            talkObjective = objective;
        }
    }
}
