using DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem
{
    /// <summary>
    /// 处理选项按钮功能的类
    /// </summary>
    public class OptionAgent : MonoBehaviour
    {
        //标题文本信息
        public Text TitleText;
        /// <summary>
        /// 选项类型
        /// </summary>
        [ReadOnly]
        public OptionType optionType;
        /// <summary>
        /// 当前的任务
        /// </summary>
        [HideInInspector]
        public Quest MQuest;
        /// <summary>
        /// 对话目标
        /// </summary>
        [HideInInspector]
        public TalkObjective talkObjective;

        [HideInInspector]
        public bool YesOrNo;

        public void Click()
        {
            switch (optionType)
            {
                case OptionType.Quest:
                    //开始任务对话
                    DialogueManager.Instance.StartQuestDialogue(MQuest);
                    break;
                case OptionType.TalkObjective:
                    //开始目标对话
                    DialogueManager.Instance.StartObjectiveDialogue(talkObjective);
                    break;
                case OptionType.Comfirm:
                    //如果是接取了任务
                    if (YesOrNo)
                    {
                        if (!MQuest.IsComplete) QuestManager.Instance.AcceptQuest(MQuest);
                        else QuestManager.Instance.CompleteQuest(MQuest);
                    }
                    DialogueManager.Instance.CloseQuestDescriptionWindow();
                    DialogueManager.Instance.GotoDefault();
                    break;
                case OptionType.Continue:
                default:
                    DialogueManager.Instance.SayNextWords();
                    break;
            }
        }
    }
    public enum OptionType
    {
        Quest,//任务
        TalkObjective,//对话目标
        Comfirm,//完成
        Continue//继续
    }
}
