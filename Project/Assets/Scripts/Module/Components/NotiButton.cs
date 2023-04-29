using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVC;
using System;

namespace Components
{
    /// <summary>
    ///  点击发送消息的按钮
    /// </summary>
    public class NotiButton : BaseMono
    {
        private Button btn;
        protected virtual void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener( Send );
        }
        /// <summary>
        ///  消息名称
        /// </summary>
        public string notiName;
        /// <summary>
        ///  消息数据
        /// </summary>
        public string data;
        /// <summary>
        ///  发送消息
        /// </summary>
        private void Send()
        {
           SendNotification( notiName , data );
        }
    }
}
