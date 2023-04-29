using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TarenaMVC;
using Common;

namespace MVC
{
    /// <summary>
    ///  封装发送消息的功能
    /// </summary>
    public class BaseMono : MonoBehaviour,INotifier
    {
        /// <summary>
        ///  发送消息
        /// </summary>
        /// <param name="name">发送的消息名</param>
        /// <param name="data">数据</param>
        public void SendNotification( string name , object data = null )
        {
            AppFacade.I.SendNotification( name , data );
        }
        /// <summary>
        ///  获取组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="name">要查找的物体名字</param>
        /// <returns></returns>
        protected T Find<T>( string name )
        {
            if ( transform.FindChildByName(name) == null )
            {
                Debug.LogError( this + " 子对象: " + name + " 没有找到!" );
                return default( T );
            }
            return transform.FindChildByName(name).GetComponent<T>();
        }
    }
}
