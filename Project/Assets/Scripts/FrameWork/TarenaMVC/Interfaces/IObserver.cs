using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  观察者接口
    /// </summary>
    public interface IObserver 
    {
        /// <summary>
        ///  处理消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="data">消息数据</param>
        void HandleNotification( string name, object data );
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        string[] ListNotificationInterests();
    }
}
