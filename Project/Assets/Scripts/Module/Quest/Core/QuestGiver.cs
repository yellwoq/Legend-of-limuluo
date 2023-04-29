using Common;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0649
namespace QuestSystem
{
    /// <summary>
    /// 任务给与者
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class QuestGiver : Talker
    {
        private SpriteRenderer mSpriteRenderer;

        [SerializeField, NonReorderable]
        private List<Quest> questInstances = new List<Quest>();
        /// <summary>
        /// 已经产生的任务集合
        /// </summary>
        public List<Quest> QuestInstances
        {
            get
            {
                return questInstances;
            }

            private set
            {
                questInstances = value;
            }
        }
        /// <summary>
        /// 任务位置偏移
        /// </summary>
        [SerializeField]
        public Vector3 questFlagOffset;

        private QuestFlag flagAgent;
        protected override void Awake()
        {
            base.Awake();
            mSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        public void Init()
        {
            InitQuest(currentTalkerInfo.QuestsStored);
            flagAgent = GameObjectPool.I.CreateObject(ID, QuestManager.Instance.QuestFlagsPrefab.gameObject, transform).GetComponent<QuestFlag>();
            flagAgent.transform.localPosition = new Vector3(0, 0.3f, 0);
            flagAgent.Init(this);
        }
        /// <summary>
        /// 使用任务信息创建任务实例
        /// </summary>
        /// <param name="questsStoraged">任务信息</param>
        public void InitQuest(List<Quest> questsStoraged)
        {
            if (questsStoraged == null) return;
            if (QuestInstances.Count > 0) QuestInstances.Clear();
            foreach (Quest quest in questsStoraged)
            {
                if (quest)
                {
                    Quest tempq = Instantiate(quest);
                    foreach (CollectObjective co in tempq.CollectObjectives)
                        tempq.Objectives.Add(co);
                    foreach (KillObjective ko in tempq.KillObjectives)
                        tempq.Objectives.Add(ko);
                    foreach (TalkObjective to in tempq.TalkObjectives)
                        tempq.Objectives.Add(to);
                    foreach (MoveObjective mo in tempq.MoveObjectives)
                        tempq.Objectives.Add(mo);
                    if (tempq.CmpltObjectiveInOrder)
                    {
                        tempq.Objectives.Sort((x, y) =>
                        {
                            if (x.OrderIndex > y.OrderIndex) return 1;
                            else if (x.OrderIndex < y.OrderIndex) return -1;
                            else return 0;
                        });
                        for (int i = 1; i < tempq.Objectives.Count; i++)
                        {
                            if (tempq.Objectives[i].OrderIndex >= tempq.Objectives[i - 1].OrderIndex)
                            {
                                tempq.Objectives[i].PrevObjective = tempq.Objectives[i - 1];
                                tempq.Objectives[i - 1].NextObjective = tempq.Objectives[i];
                            }
                        }
                    }
                    int i1, i2, i3, i4;
                    i1 = i2 = i3 = i4 = 0;
                    foreach (Objective o in tempq.Objectives)
                    {
                        if (o is CollectObjective)
                        {
                            o.runtimeID = tempq._ID + "_CO" + i1;
                            i1++;
                        }
                        if (o is KillObjective)
                        {
                            o.runtimeID = tempq._ID + "_KO" + i2;
                            i2++;
                        }
                        if (o is TalkObjective)
                        {
                            o.runtimeID = tempq._ID + "_TO" + i3;
                            i3++;
                        }
                        if (o is MoveObjective)
                        {
                            o.runtimeID = tempq._ID + "_MO" + i4;
                            i4++;
                        }
                    }
                    tempq.MOriginQuestGiver = this;
                    tempq.MCurrentQuestGiver = this;
                    QuestInstances.Add(tempq);
                }
            }
        }
        /// <summary>
        /// 向此对象交接任务。因为往往会有些任务不在同一个NPC接取并完成，所以就要在两个NPC之间交接该任务
        /// </summary>
        /// <param name="quest">需要进行交接的任务</param>
        public void TransferQuestToThis(Quest quest)
        {
            if (!quest) return;
            QuestInstances.Add(quest);
            quest.MCurrentQuestGiver.QuestInstances.Remove(quest);
            quest.MCurrentQuestGiver = this;
        }
        /// <summary>
        /// 移除所有与该对话者相关的目标
        /// </summary>
        /// <param name="objective"></param>
        /// <param name="befCmplt"></param>
        public void TryRemoveObjective(Objective objective, bool befCmplt)
        {
            if (!befCmplt && objective.IsComplete)
                if (objective is TalkObjective)
                    if (talkToThisObjectives.FindAll(x=>x.talkObjective==objective as TalkObjective)!=null)
                        talkToThisObjectives.RemoveAll(x => x.talkObjective == objective as TalkObjective);
        }
        private void OnDestroy()
        {
            if (flagAgent) flagAgent.Recycle();
        }
    }
}
