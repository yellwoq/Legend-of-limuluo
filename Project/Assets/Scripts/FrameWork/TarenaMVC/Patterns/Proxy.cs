using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    /// 处理数据基类
    /// </summary>
    public class Proxy : IProxy
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public const string NAME = "Proxy";
        /// <summary>
        ///  构造函数
        /// </summary>
        public Proxy()
        {
            this.ProxyName = NAME;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string ProxyName { get; set; }
        /// <summary>
        ///  发送消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="data">消息数据</param>
        public void SendNotification( string name , object data = null )
        {
            Facade.I.SendNotification( name , data );
        }
    }
}
