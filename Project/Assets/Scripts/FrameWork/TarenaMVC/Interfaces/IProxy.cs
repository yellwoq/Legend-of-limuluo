using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  数据处理接口
    /// </summary>
    public interface IProxy :INotifier
    {
        /// <summary>
        ///  Proxy的名称
        /// </summary>
        string ProxyName { get; set; }
    }
}
