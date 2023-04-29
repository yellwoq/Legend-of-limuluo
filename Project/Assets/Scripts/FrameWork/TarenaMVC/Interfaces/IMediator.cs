using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  UI和数据之间的中介者
    /// </summary>
    public interface IMediator:IObserver,INotifier
    {
        /// <summary>
        ///  Mediator的名称
        /// </summary>
        string MediatorName { get; set; }
    }
}
