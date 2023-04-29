using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  注册面板
    /// </summary>
    public class RegisterPanel : MonoBehaviour,IObserver
    {
        /// <summary>
        ///  处理消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public void HandleNotification( string name , object data )
        {
            Debug.Log( this + " HandleNotification: " + name );
            switch ( name )
            {
                case "ShowLogin":
                    this.gameObject.SetActive( false );
                    break;
                case "ShowRegister":
                    this.gameObject.SetActive( true );
                    break;
            }
        }
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public string[] ListNotificationInterests()
        {
            return new string[] { "ShowLogin" , "ShowRegister" };
        }

        private Button returnButton;
        // Use this for initialization
        void Awake()
        {
            returnButton = transform.Find( "returnButton" ).GetComponent<Button>();
            // returnButton
            returnButton.onClick.AddListener( ShowLogin );
            // 添加到消息中心
            NotificationCenter.I.AddObserver( this );
            // 自动隐藏
            this.gameObject.SetActive( false );
        }
        /// <summary>
        ///  显示登录面板
        /// </summary>
        private void ShowLogin()
        {
            // 发送 "ShowLogin" 消息
            NotificationCenter.I.SendNotification( "ShowLogin" );
        }
    }
}
