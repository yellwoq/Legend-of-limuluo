using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 有限状态机基类
    /// </summary>
    public class FsmBase<T> : MonoBehaviour where T: FsmBase<T>
    {
        /// <summary>
        /// 状态列表
        /// </summary>
        private List<FsmState<T>> stateList;

        [DisplayName("配置文件名称")]
        public string configName;

        protected void Awake()
        {
            ConfigFSM();
            InitDefaultState();
        }

        /// <summary>
        /// 配置有限状态机
        /// </summary>
        private void ConfigFSM()
        {
            //读取配置文件
            //形成数据结构
            var map = new FSMConfigReader(configName).map;
            //配置有限状态机
            stateList = new List<FsmState<T>>();
            foreach (string mainKey in map.Keys)
            {
                Type type = Type.GetType("AI.FSM." + mainKey + "State");
                FsmState<T> state = Activator.CreateInstance(type) as FsmState<T>;
                state.Fsm = this as T ;
                foreach (var subMap in map[mainKey])
                { 
                    state.AddMap(subMap.Key, subMap.Value);
                }
                stateList.Add(state);
            }
        }

        [DisplayName("默认状态")]
        public string defaultStateName;
        /// <summary>
        /// 当前状态
        /// </summary>
        private FsmState<T> currentState;

        private FsmState<T> defaultState;
        /// <summary>
        /// 初始化默认状态
        /// </summary>
        protected void InitDefaultState()
        {
            //在状态列表中查找默认状态
            defaultState = stateList.Find(e => e.GetType().Name == defaultStateName + "State");
            currentState = defaultState;
            currentState.OnStateEnter();
        }

        protected void Update()
        {
            currentState.OnStateStay();
            string nextStateName = currentState.Check();
            if (nextStateName != null)
            {
                ChangeState(nextStateName);
            }
        }
        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateName"></param>
        private void ChangeState(string stateName)
        {
            //离开之前状态
            currentState.OnStateExit();
            //切换
            if (stateName == "Default")
                currentState = defaultState;
            else
               currentState = stateList.Find(e => e.GetType().Name == stateName+"State");
            //进入当前状态
            currentState.OnStateEnter();
        }
    }
}