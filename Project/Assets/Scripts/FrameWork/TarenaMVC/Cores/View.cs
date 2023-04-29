using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  管理IMediator，操作具体的视图组件
    /// </summary>
    public class View : Singleton<View>, IView
    {
        protected override void Initialize()
        {
            mediatorMap = new Dictionary<string , IMediator>();
        }
        /// <summary>
        ///  存储IMediator
        /// </summary>
        private Dictionary<string , IMediator> mediatorMap;
        /// <summary>
        ///  注册IMediator
        /// </summary>
        /// <param name="mediator"></param>
        public void RegisterMediator( IMediator mediator )
        {
            mediatorMap[ mediator.MediatorName ] = mediator;
            // 添加观察者
            NotificationCenter.I.AddObserver( mediator );
        }
        /// <summary>
        ///  获取IMediator
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMediator RetrieveMediator( string name )
        {
            IMediator mediator = mediatorMap.ContainsKey( name ) ?
                mediatorMap[ name ] : null;
            return mediator;
        }
        /// <summary>
        ///  移除IMediator
        /// </summary>
        /// <param name="name"></param>
        public void RemoveMediator( string name )
        {
            IMediator mediator = mediatorMap.ContainsKey( name ) ?
                mediatorMap[ name ] : null;
            // 移除观察者
            if ( mediator != null )
                NotificationCenter.I.RemoveObserver( mediator );
            mediatorMap.Remove( name );
        }
    }
}
