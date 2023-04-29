using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace QuestSystem
{
    /// <summary>
    ///  单个处理任务，以在列表中用任务名称显示任务，并在点击时弹出任务详情
    /// </summary>
    public class QuestAgent : MonoBehaviour
    {
        /// <summary>
        /// 当前的任务
        /// </summary>
        [HideInInspector]
        public Quest MQuest;

        public Text TitleText;

        public void UpdateQuestStatus()
        {
            if (MQuest)
            {
                StringBuilder @string = new StringBuilder();
                if (MQuest.MOriginQuestGiver.ID == "NPC000") { @string.Append("<size=17><color=yellow>主任务</color></size>"); }
                @string.Append(MQuest.Title + (MQuest.IsComplete ? "(完成)" : string.Empty));
                TitleText.text = @string.ToString();
            }
        }

        public void Click()
        {
            if (!MQuest) return;
            QuestManager.Instance.ShowDescription(MQuest);
            QuestManager.Instance.OpenDescriptionWindow(this);
        }
    }
}
