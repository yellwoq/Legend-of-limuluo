using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  管理IProxy，数据相关
    /// </summary>
    public class Model : Singleton<Model>,IModel
    {
        protected override void Initialize()
        {
            proxyMap = new Dictionary<string , IProxy>();
        }
        /// <summary>
        ///  存储IProxy
        /// </summary>
        private Dictionary<string , IProxy> proxyMap;
        /// <summary>
        ///  注册IProxy
        /// </summary>
        /// <param name="proxy"></param>
        public void RegisterProxy( IProxy proxy )
        {
            proxyMap[ proxy.ProxyName ] = proxy;
        }
        /// <summary>
        ///  获取IProxy
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IProxy RetrieveProxy( string name )
        {
            IProxy proxy = proxyMap.ContainsKey( name ) ?
                proxyMap[ name ] : null;
            return proxy;
        }
        /// <summary>
        ///  移除IProxy
        /// </summary>
        /// <param name="name"></param>
        public void RemoveProxy( string name )
        {
            proxyMap.Remove( name );
        }
    }
}
