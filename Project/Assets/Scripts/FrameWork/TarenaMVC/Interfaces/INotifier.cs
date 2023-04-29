using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  发送消息
    /// </summary>
    public interface INotifier 
    {
        /// <summary>
        ///  发送消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="data">消息数据</param>
        void SendNotification( string name , object data );
    }
}
