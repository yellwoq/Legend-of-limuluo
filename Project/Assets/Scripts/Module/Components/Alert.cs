using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using UnityEngine.Events;
using System;

namespace Components
{
    /// <summary>
    ///  全局弹出框
    /// </summary>
    public class Alert : BasePanel
    {
        /// <summary>
        /// 标题
        /// </summary>
        private Text titleText;
        /// <summary>
        /// 内容
        /// </summary>
        private Text contentText;
        /// <summary>
        /// 确认按钮
        /// </summary>
        private Button okButton;

        protected override void Awake()
        {
            base.Awake();
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                instance = this;
            }
            titleText = Find<Text>( "titleText" );
            contentText = Find<Text>( "contentText" );
            okButton = Find<Button>( "okButton" );
            okButton.onClick.AddListener( TryCallback );
            // 自动隐藏
            Hide();
        }
        /// <summary>
        ///  回调函数
        /// </summary>
        private UnityAction<object> callback;
        /// <summary>
        ///  参数
        /// </summary>
        private object data;
        /// <summary>
        ///  调用回调函数
        /// </summary>
        private void TryCallback()
        {
            // 关闭
            Hide();
            // 调用回调函数
            if ( callback != null ) callback( data );           
        }
        /// <summary>
        ///  显示弹出框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="callback">回调函数</param>
        /// <param name="data">参数</param>
        /// <param name="isShowCloseButton">是否显示关闭按钮</param>
        public void ShowIt( string title, string content,
            UnityAction<object> callback=null, object data=null,
            bool isShowCloseButton = true )
        {
            Debug.Log( "ShowAlert: " + title + " " + content + " " + callback );
            this.titleText.text = title;
            this.contentText.text = content;
            this.callback = callback;
            this.data = data;
            closeButton.gameObject.SetActive(isShowCloseButton);
            Show();
        }

        /// <summary>
        ///  静态属性
        /// </summary>
        public static Alert instance;
        /// <summary>
        ///  显示弹出框
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">内容</param>
        /// <param name="callback">回调函数</param>
        /// <param name="data">参数</param>
        /// <param name="isShowCloseButton">是否显示关闭按钮</param>
        public static void Show( string title , string content ,
            UnityAction<object> callback = null , object data = null ,
            bool isShowCloseButton = true )
        {
            instance.ShowIt( title , content , callback , data , isShowCloseButton);
        }
    }
}
