using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  UI和数据之间的中介者
    /// </summary>
    public class Mediator : IMediator
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public const string NAME = "Mediator";
        /// <summary>
        ///  构造函数
        /// </summary>
        public Mediator()
        {
            this.MediatorName = NAME;
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string MediatorName { get; set; }
        /// <summary>
        ///  处理消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public virtual void HandleNotification( string name , object data )
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public virtual string[] ListNotificationInterests()
        {
            throw new System.NotImplementedException();
        }
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
