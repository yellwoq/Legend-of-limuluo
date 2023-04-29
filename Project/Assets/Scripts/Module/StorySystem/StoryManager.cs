using Common;
using Fungus;
using QuestSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StorySystem
{
    public class StoryManager : MonoSingleton<StoryManager>
    {
        /// <summary>
        /// 所有的剧情信息
        /// </summary>
        public List<StoryAgent> StoryList;
        [DisplayName("玩家交互界面画布")]
        public GameObject heroCanvas;
        [DisplayName("游戏操作界面画布")]
        public GameObject gameCanvas;
        [DisplayName("要动态添加的命令")]
        public List<MyCommand> CommandList;
        /// <summary>
        /// 初始化剧情信息
        /// </summary>
        public void InitStory()
        {
            StoryList.Clear();
            StoryList.AddRange(FindObjectsOfType<StoryAgent>());
            if (StoryList != null)
            {
                Debug.Log(StoryList.Count);
                //如果正处于有开始剧情的场景并且还没有经过
                Flowchart startFC = StoryList.Find(e => { return e.FlowChatID == "GameStart000"; }).CurrentFlowchart;
                if (startFC != null)
                {
                    //如果还没有经历这段剧情
                    if (!startFC.GetBooleanVariable("isHasRead"))
                    {
                        GameStartAddBlock();
                        StartCoroutine(GameExecuted(startFC));
                    }
                    else
                    {
                        FindObjectOfType<SystemQuestGiver>().GiveQuest();
                    }
                }
            }
        }
        /// <summary>
        /// 设置剧情信息
        /// </summary>
        /// <param name="flowchartID"></param>
        /// <param name="isActive"></param>
        public void SetStory(string flowchartID, bool isHasRead)
        {
            if (StoryList == null) return;
            StoryAgent currentSA = StoryList.Find(e => { return e.FlowChatID == flowchartID; });
            Flowchart currentFC = currentSA.CurrentFlowchart;
            currentFC.SetBooleanVariable("isHasRead", isHasRead);
            Debug.Log(flowchartID);
            //如果还没有阅读
            if (!isHasRead)
            {
                //如果是开始节点.添加节点
                if (flowchartID == "GameStart000")
                {
                    GameStartAddBlock();
                }
                //发送消息
                MessageReceived[] receivers = null;
                if (currentSA.MyMessageTarget == MessageTarget.SameFlowchart)
                {
                    receivers = GetComponents<MessageReceived>();
                }
                else
                {
                    receivers = FindObjectsOfType<MessageReceived>();
                }

                if (receivers != null)
                {
                    for (int i = 0; i < receivers.Length; i++)
                    {
                        var receiver = receivers[i];
                        receiver.OnSendFungusMessage(currentSA.Message.Value);
                    }
                }
                //等待执行结束
                StartCoroutine(GameExecuted(currentFC));

            }
        }
        /// <summary>
        /// 游戏开始判断是否添加节点
        /// </summary>
        public bool GameStartAddBlock()
        {
            if (StoryList == null) return false;
            //如果正处于有开始剧情的场景并且还没有经过
            Flowchart startFC = StoryList.Find(e => { return e.FlowChatID == "GameStart000"; }).CurrentFlowchart;
            //动态添加节点
            Block dynamicBlock = startFC.CreateBlock(Vector2.one);
            dynamicBlock.BlockName = "DN_StartGame";

            MyMessageReceived handle = startFC.gameObject.AddComponent<MyMessageReceived>();
            handle.Message = "StartGame";
            handle.ParentBlock = dynamicBlock;
            dynamicBlock._EventHandler = handle;

            foreach (MyCommand command in CommandList)
            {
                switch (command.commandType)
                {
                    case CommandType.SetCanvasGroup:
                        AddComandDynamic(startFC, dynamicBlock, (SetCanvasGroup groupCommand, List<CommandContent> contents) =>
                        {

                            foreach (var content in contents)
                            {
                                switch (content.varType)
                                {
                                    case VariableType.Float:
                                        groupCommand.alphaValue = float.Parse(content.contentValue);
                                        break;
                                    case VariableType.GameObject:
                                        groupCommand.canvasGroup = new CanvasGroup[] { heroCanvas.GetComponent<CanvasGroup>(), gameCanvas.GetComponent<CanvasGroup>() }.Find(
                                        (e) => { return e.name == content.contentValue; });
                                        break;
                                }
                            }

                        }, command.contentList);
                        break;
                }
            }
            if (dynamicBlock.CommandList.Count > 0)
                startFC.AddSelectedCommand(dynamicBlock.CommandList[0]);
            startFC.AddSelectedBlock(dynamicBlock);
            return true;
        }

        public IEnumerator GameExecuted(Flowchart flowchart)
        {
            yield return new WaitForSeconds(1);
            yield return new WaitUntil(() => { return flowchart.GetExecutingBlocks().Count <= 0;});
            FindObjectOfType<SystemQuestGiver>().GiveQuest();
        }
        /// <summary>
        /// 动态添加节点
        /// </summary>
        /// <typeparam name="T">节点类型</typeparam>
        /// <param name="parentFlowchart">父剧情窗口</param>
        /// <param name="parentBlock">父节点</param>
        /// <param name="commangHandle">命令处理逻辑</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandContent">命令内容</param>
        public void AddComandDynamic<T>(Flowchart parentFlowchart, Block parentBlock, UnityAction<T, List<CommandContent>> commangHandle, List<CommandContent> commandContents) where T : Command
        {
            T currentCommand = parentFlowchart.gameObject.AddComponent<T>();
            currentCommand.ParentBlock = parentBlock;
            currentCommand.ItemId = parentFlowchart.NextItemId();
            currentCommand.CommandIndex = parentBlock.CommandList.Count;
            currentCommand.OnCommandAdded(parentBlock);
            commangHandle(currentCommand, commandContents);
            parentBlock.CommandList.Add(currentCommand);
        }
    }

}
