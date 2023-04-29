using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TarenaMVC
{
    /// <summary>
    ///  登录面板
    /// </summary>
    public class LoginPanel : MonoBehaviour,IObserver
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
                    this.gameObject.SetActive( true );
                    break;
                case "ShowRegister":
                    this.gameObject.SetActive( false );
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

        private Button registerButton;
        // Use this for initialization
        void Awake()
        {        
            registerButton = transform.Find( "registerButton" ).GetComponent<Button>();
            // 监听registerButton点击
            registerButton.onClick.AddListener( ShowRegister );
           

            // 添加到消息中心
            NotificationCenter.I.AddObserver( this );
        }

        private void ShowRegister()
        {
            // 发送 "ShowRegister" 消息
            NotificationCenter.I.SendNotification( "ShowRegister" );
        }
    }
}
