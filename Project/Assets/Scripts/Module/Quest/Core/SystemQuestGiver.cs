using System;
using Common;
using DialogueSystem;
using UnityEngine;

namespace QuestSystem
{
    /// <summary>
    /// 系统任务给与者
    /// </summary>
    [DisallowMultipleComponent]
    public class SystemQuestGiver : QuestGiver
    {
        private static bool dontDestroyOnLoadOnce;

        protected override void Awake()
        {
            base.Awake();
            if (!dontDestroyOnLoadOnce)
            {
                DontDestroyOnLoad(this);
                dontDestroyOnLoadOnce = true;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        public void GiveQuest()
        {
            for (int i=0;i<QuestInstances.Count;i++)
            {
                Debug.Log(QuestInstances[i].Title + ":" + QuestInstances[i].AcceptAble);
                if (!QuestInstances[i].IsComplete && QuestInstances[i].AcceptAble)
                {
                    QuestManager.I.AcceptQuest(QuestInstances[i]);
                }
            }
        }

        public static explicit operator GameObject(SystemQuestGiver v)
        {
            return v.gameObject;
        }
    }
}
