using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 状态类：代表具体状态，隔离状态机与状态的变化
    /// </summary>
    public class FsmState<T>
    {
        //条件列表
        private List<FsmTrigger<T>> triggerList;

        //策划配置的映射表(条件  -->  状态)
        private Dictionary<string, string> map;
        /// <summary>
        /// 状态机
        /// </summary>
        [HideInInspector]
        public T Fsm;

        public FsmState()
        {
            triggerList = new List<FsmTrigger<T>>();
            map = new Dictionary<string, string>();
        }

        /// <summary>
        /// 添加映射
        /// </summary>
        /// <param name="triggerName">条件名</param>
        /// <param name="stateName">状态名</param>
        public void AddMap(string triggerName,string stateName)
        {
            map.Add(triggerName, stateName);

            Type type = Type.GetType("AI.FSM." + triggerName + "Trigger");
            FsmTrigger<T> trigger = Activator.CreateInstance(type) as FsmTrigger<T>;
            //为条件提供状态机引用
            trigger.Fsm = Fsm;
            triggerList.Add(trigger);
        }

        /// <summary>
        /// 检查条件
        /// </summary>
        /// <returns></returns>
        public string Check()
        {
            for (int i = 0; i < triggerList.Count; i++)
            {
                if (triggerList[i].OnTriggerHandler())
                {
                    //发现满足的条件
                    //获取条件对象类名
                    //策划配置：NoHealth
                    //程序类名：NoHealthTrigger
                    string triggerClassName = triggerList[i].GetType().Name;
                    string stateName = map[triggerClassName.Replace("Trigger","")];
                    return stateName;
                }
            }
            return null;
        }

        /// <summary>
        /// 当状态进入时逻辑
        /// </summary>
        public virtual void OnStateEnter() { }
        /// <summary>
        /// 当状态停留时逻辑
        /// </summary>
        public virtual void OnStateStay() { }
        /// <summary>
        /// 当状态离开时逻辑
        /// </summary>
        public virtual void OnStateExit() { } 
    }
}