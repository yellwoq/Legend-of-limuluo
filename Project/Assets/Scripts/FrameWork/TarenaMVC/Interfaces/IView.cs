using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  管理IMediator
    /// </summary>
    public interface IView
    {
        /// <summary>
        ///  注册IMediator
        /// </summary>
        /// <param name="proxy"></param>
        void RegisterMediator( IMediator mediator );
        /// <summary>
        ///  获取IMediator
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMediator RetrieveMediator( string name );
        /// <summary>
        ///  移除IMediator
        /// </summary>
        /// <param name="name"></param>
        void RemoveMediator( string name );
    }
}
