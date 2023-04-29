using Fungus;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0649
namespace StorySystem
{
    /// <summary>
    /// 剧情信息
    /// </summary>
    public class StoryAgent : MonoBehaviour
    {
        [DisplayName("剧情ID"), SerializeField]
        private string flowChartID = null;
        public string FlowChatID => flowChartID;
        [DisplayName("剧情名"), SerializeField]
        private string storyName;
        public string StoryName => storyName;
        private Flowchart currentFlowchart;
        public Flowchart CurrentFlowchart
        {
            get
            {
                Flowchart currentFC = GetComponent<Flowchart>();
                if (!currentFC.HasVariable("isHasRead"))
                {
                    BooleanVariable isHasReadVar = new BooleanVariable();
                    isHasReadVar.Key = "isHasRead";
                    isHasReadVar.Scope = VariableScope.Public;
                    isHasReadVar.Value = false;
                    currentFC.Variables.Add(isHasReadVar);
                }
                return currentFC;
            }
        }
        [DisplayName("该段剧情是否完成"), SerializeField]
        private bool isComplete = false;
        public bool IsComplete
        {
            get { return isComplete; }
            set { IsComplete = value; }
        }

        [DisplayName("发送消息的目标类型"), SerializeField]
        private MessageTarget messageTarget = MessageTarget.SameFlowchart;
        public MessageTarget MyMessageTarget
        {
            get
            {
                return messageTarget;
            }
        }
        [DisplayName("发送的消息"), SerializeField]
        private StringData _message;
        public StringData Message
        {
            get
            {
                return _message;
            }
        }
        [SerializeField, DisplayName("要动态添加的命令")]
        private List<MyCommand> commandList;

        public void SetStoryIsComlete(bool isCompleted)
        {
            isComplete = isCompleted;
        }
    }
}
