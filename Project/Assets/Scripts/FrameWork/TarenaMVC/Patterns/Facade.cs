using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  程序入口基类
    /// </summary>
    public class Facade :Singleton<Facade>, IFacade
    {
        /// <summary>
        ///  注册Mediator
        /// </summary>
        /// <param name="mediator"></param>
        public void RegisterMediator( IMediator mediator )
        {
            View.I.RegisterMediator( mediator );
        }
        /// <summary>
        ///  注册Proxy
        /// </summary>
        /// <param name="proxy"></param>
        public void RegisterProxy( IProxy proxy )
        {
            Model.I.RegisterProxy( proxy );
        }
        /// <summary>
        ///  移除Mediator
        /// </summary>
        /// <param name="name"></param>
        public void RemoveMediator( string name )
        {
            View.I.RemoveMediator( name );
        }
        /// <summary>
        ///  移除Proxy
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProxy( string name )
        {
            Model.I.RemoveProxy( name );
        }
        /// <summary>
        ///  获取Mediator
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMediator RetrieveMediator( string name )
        {
            return View.I.RetrieveMediator( name );
        }
        /// <summary>
        ///  获取Proxy
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IProxy RetrieveProxy( string name )
        {
            return Model.I.RetrieveProxy( name );
        }
        /// <summary>
        ///  发送消息
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="data">消息数据</param>
        public void SendNotification( string name , object data = null)
        {
            NotificationCenter.I.SendNotification( name , data );
        }
    }
}
