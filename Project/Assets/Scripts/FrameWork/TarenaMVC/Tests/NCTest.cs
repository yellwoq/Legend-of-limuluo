using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  消息中心测试类
    /// </summary>
    public class NCTest : MonoBehaviour,IObserver
    {
        /// <summary>
        ///  处理消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void HandleNotification( string name , object data )
        {
            Debug.Log( this + " HandleNotification: " + name );
        }
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public string[] ListNotificationInterests()
        {
            return new string[] { "Login" , "Register" };
        }

        // Use this for initialization
        void Start()
        {
            NotificationCenter.I.AddObserver( this ); // 添加
            NotificationCenter.I.PrintReaderMap();
            NotificationCenter.I.SendNotification( "Login" ); // 发送
        }

    }
}
