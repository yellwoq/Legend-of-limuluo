using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 条件基类:代表具体条件，隔离状态与条件的变化。
    /// </summary>
    public abstract class FsmTrigger<T>
    {
        //子类可以直接使用的成员
        public T Fsm;
        public abstract bool OnTriggerHandler();
    }
}