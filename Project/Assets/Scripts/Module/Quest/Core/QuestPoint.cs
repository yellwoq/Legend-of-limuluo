using Components;
using Fungus;
using StorySystem;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0649
namespace QuestSystem
{
    public delegate void MoveToPointListener(QuestPoint point);
    /// <summary>
    /// 任务点
    /// </summary>
    public class QuestPoint : MonoBehaviour
    {

        [SerializeField, DisplayName("任务点ID")]
        private string ID;
        /// <summary>
        /// 任务点id
        /// </summary>
        public string _ID
        {
            get
            {
                return ID;
            }
        }
        [DisplayName("是否能执行故事")]
        public bool canExcuteStory;
        [ConditionalHide("目标故事情节", "canExcuteStory", true)]
        public StoryAgent targetStory;
        [ConditionalHide("开始节点名", "canExcuteStory", true)]
        public string startBlockName;
        public event MoveToPointListener OnMoveIntoEvent;
        public event MoveToPointListener OnMoveAwayEvent;
        Flowchart targetFC;
        private void OnEnable() { }
        private void OnTriggerEnter(Collider other)
        {
            OnMoveIntoEvent?.Invoke(this);
        }
        private void OnTriggerExit(Collider other)
        {
            OnMoveAwayEvent?.Invoke(this);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnMoveIntoEvent?.Invoke(this);
            if (targetStory)
            {
                targetFC = targetStory.GetComponent<Flowchart>();
                if (canExcuteStory && isQuestNeedPoint() && !targetFC.GetBooleanVariable("isHasRead"))
                {
                    StartCoroutine(StoryCheckStart());
                }
                FindObjectOfType<SystemQuestGiver>().GiveQuest();
            }
        }
        private System.Collections.IEnumerator StoryCheckStart()
        {

            yield return new WaitUntil(() => !Alert.instance.gameObject.activeSelf);
            if(!targetFC.HasExecutingBlocks())
            targetFC.ExecuteBlock(startBlockName);
            yield return new WaitUntil(() => targetFC.GetBooleanVariable("isHasRead"));
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            OnMoveAwayEvent?.Invoke(this);
        }
        /// <summary>
        /// 是否有任务需要
        /// </summary>
        /// <returns></returns>
        public bool isQuestNeedPoint()
        {
            foreach (var quest in QuestManager.I.QuestsOngoing)
            {
                foreach (var moveObjective in quest.MoveObjectives)
                {
                    if (moveObjective.PointID == ID)
                    {
                        return true;
                    }

                }
            }
            List<Quest> quests = QuestManager.I.allQuests;
            if (quests != null)
            {
                //从未接取的任务且未完成的任务中，其他的任务条件都已经满足而只剩本类条件未满足的任务中查找是否有需要
                bool isOnlyThisCondition = quests.FindAll(x =>
                  {
                      if (x.AcceptConditions != null)
                      {
                          bool judgeCanExicute = true;
                          foreach (var condition in x.AcceptConditions)
                          {
                              if (!condition.IsEligible && condition.ConditionType != ConditionType.StoryPlay)
                              {
                                  judgeCanExicute = false;
                                  break;
                              }
                          }
                          return judgeCanExicute;

                      }
                      else
                      {
                          bool judgeCanExicute = !QuestManager.I.QuestsOngoing.Contains(x);
                          judgeCanExicute &= !QuestManager.I.QuestsComplete.Contains(x);
                          return judgeCanExicute;
                      }
                  }).Count > 0;
                return isOnlyThisCondition;
            }
            return false;
        }
    }
}
