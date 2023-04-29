using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  管理IProxy
    /// </summary>
    public interface IModel
    {
        /// <summary>
        ///  注册IProxy
        /// </summary>
        /// <param name="proxy">proxy参数</param>
        void RegisterProxy( IProxy proxy );
        /// <summary>
        ///  获取IProxy
        /// </summary>
        /// <param name="name">proxy名字</param>
        /// <returns></returns>
        IProxy RetrieveProxy( string name );
        /// <summary>
        ///  移除IProxy
        /// </summary>
        /// <param name="name">proxy名字</param>
        void RemoveProxy( string name );
    }
}
