using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Components;
using UI;
using Common;

namespace MVC
{
    /// <summary>
    ///  用户登录面板
    /// </summary>
    public class LoginPanel : BasePanel
    {
        // 初始化组件
        private InputField userInput;
        private InputField pwdInput;
        private Button loginButton;
        private Button registerButton;
        //是否记住密码
        bool isOn = false;
        protected override void Awake()
        {
            userInput = Find<InputField>( "userInput" );
            pwdInput = Find<InputField>( "pwdInput" );
            loginButton = Find<Button>( "loginButton" );
            loginButton.onClick.AddListener( Login );
            // 显示注册面板按钮
            registerButton = Find<Button>( "registerButton" );
            registerButton.onClick.AddListener( ShowRegister );
            // 记住密码选中状态
            isOn = PlayerPrefs.HasKey( KeyList.REMEMBER_PWD )
                && PlayerPrefs.GetString( KeyList.REMEMBER_PWD ) == "true";
            //pwdToggle.isOn = isOn;
            // 读取记住的用户名和密码
            if( isOn && PlayerPrefs.HasKey( KeyList.USERNAME ) )
            {
                userInput.text = PlayerPrefs.GetString( KeyList.USERNAME );
                pwdInput.text = PlayerPrefs.GetString( KeyList.PWD );
            }
            Sound.SoundManager.I.PlayBgm("Title");
        }
        private void OnEnable()
        {
            if (isOn && PlayerPrefs.HasKey(KeyList.USERNAME))
            {
                userInput.text = PlayerPrefs.GetString(KeyList.USERNAME);
                pwdInput.text = PlayerPrefs.GetString(KeyList.PWD);
            }
        }
        /// <summary>
        /// 显示注册面板
        /// </summary>
        private void ShowRegister()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            SendNotification( NotiList.SHOW + NotiList.REGISTER );
        }

        /// <summary>
        ///  登录
        /// </summary>
        private void Login()
        {
            //点击音效
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            // 检查用户名和密码是否为空
            if ( userInput.text == string.Empty ||
                pwdInput.text == string.Empty )
            {
                Debug.LogError( "用户名和密码均不能为空!" );
                // 弹出框
                Alert.Show( "登录错误" , "用户名和密码均不能为空!" );
                return;
            }
            // 检查是否有非法字符
            if( !StringHelper.IsSafeSqlString( userInput.text)
                || StringHelper.CheckBadWord( userInput.text) 
                || !StringHelper.IsSafeSqlString( pwdInput.text )
                || StringHelper.CheckBadWord( pwdInput.text ) )
            {
                Debug.LogError( "用户名和密码均不能有非法字符!" );
                // 弹出框
                Alert.Show( "登录错误" , "用户名和密码均不能有非法字符!" );
                return;
            }
            // UserVO
            UserVO user = new UserVO( userInput.text , pwdInput.text );
            // 发送 登录 消息
            SendNotification( NotiList.LOGIN , user );
        }
    }
}
