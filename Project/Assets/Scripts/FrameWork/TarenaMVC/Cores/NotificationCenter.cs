using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TarenaMVC
{
    /// <summary>
    ///  消息中心
    /// </summary>
    public class NotificationCenter : Singleton<NotificationCenter>
    {
        /// <summary>
        ///  初始化
        /// </summary>
        protected override void Initialize()
        {
            observerMap = new Dictionary<string , List<IObserver>>();
        }
        /// <summary>
        ///  观察者列表
        /// </summary>
        private Dictionary<string , List<IObserver>> observerMap;
        /// <summary>
        ///  添加观察者
        /// </summary>
        /// <param name="name">消息名称</param>
        /// <param name="observer">观察者</param>
        public void AddObserver( string name, IObserver observer )
        {
            // 判断报纸列表有没有
            if ( !observerMap.ContainsKey( name ) ) // 如果没有
                observerMap.Add( name , new List<IObserver>() ); // 加一个List
            // 观察者加到List里面
            observerMap[ name ].Add( observer );
        }
        /// <summary>
        ///  添加观察者
        /// </summary>
        /// <param name="observer">观察者</param>
        public void AddObserver( IObserver observer )
        {
            string[] list = observer.ListNotificationInterests();
            for ( int i = 0; i < list.Length; i++ )
            {
                AddObserver( list[ i ] , observer );
            }
        }
        /// <summary>
        ///  移除观察者
        /// </summary>
        /// <param name="name">消息</param>
        /// <param name="observer">要移除的观察者</param>
        public void RemoveObserver( string name, IObserver observer )
        {
            // 没有监听消息的list,直接返回
            if ( !observerMap.ContainsKey( name ) ) return;
            // list里面没有observer,直接返回
            if ( !observerMap[ name ].Contains( observer ) ) return;
            // 移除
            observerMap[ name ].Remove( observer );
            // 判断list是否为空
            if ( observerMap[ name ].Count == 0 )
                observerMap.Remove( name );
        }
        /// <summary>
        ///  从所有消息中移除观察者
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver( IObserver observer )
        {
            string[] list = observer.ListNotificationInterests();
            for ( int i = 0; i < list.Length; i++ )
            {
                RemoveObserver( list[ i ] , observer );
            }
        }
        /// <summary>
        ///  发送消息
        /// </summary>
        /// <param name="name">发送的消息名</param>
        /// <param name="data">数据</param>
        public void SendNotification( string name , object data = null)
        {
            Debug.Log( "SendNotification:: " + name + " data: " + data );
            // 如果没有人订阅,直接返回
            if ( !observerMap.ContainsKey( name ) ) return;
            // 找出订阅列表
            List<IObserver> list = observerMap[ name ];
            // 遍历
            for ( int i = 0; i < list.Count; i++ )
            {
                list[ i ].HandleNotification( name , data);  // 挨个送
            }          
        }
        /// <summary>
        ///  打印观察者列表
        /// </summary>
        public void PrintReaderMap()
        {
            string s = "--------------开始打印-------------\n";
            foreach( string name in observerMap.Keys )
            {
                s += name + " : [ ";
                List<IObserver> list = observerMap[ name ];
                for ( int i = 0; i < list.Count; i++ )
                {
                    s += list[ i ];
                    if ( i != list.Count - 1 ) s += " , ";
                }
                s += " ] \n";
            }
            s += "--------------开始结束-------------\n";
            Debug.Log( s );
        }
    }
}

