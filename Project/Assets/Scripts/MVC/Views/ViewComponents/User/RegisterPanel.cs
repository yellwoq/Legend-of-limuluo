using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using System;
using Components;

namespace MVC
{
    /// <summary>
    ///  注册面板
    /// </summary>
    public class RegisterPanel : BasePanel
    {
        private InputField userInput;
        private InputField pwdInput;
        private InputField pwdInput2;
        private Button registerButton;
        private Button returnButton;

        protected override void Awake()
        {
            base.Awake();
            // 获取组件
            userInput = Find<InputField>( "userInput" );
            pwdInput = Find<InputField>( "pwdInput" );
            pwdInput2 = Find<InputField>( "pwdInput2" );
            registerButton = Find<Button>( "registerButton" );
            registerButton.onClick.AddListener( Register );
            // 返回按钮
            returnButton = Find<Button>( "returnButton" );
            returnButton.onClick.AddListener( ShowLogin );
            // closeButton
            closeButton.onClick.AddListener( ShowLogin );
        }

        /// <summary>
        ///  显示登录面板
        /// </summary>
        private void ShowLogin()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            SendNotification( NotiList.SHOW + NotiList.LOGIN );
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public void ReflashPanel()
        {
            userInput.text=string.Empty;
            pwdInput.text = string.Empty;
            pwdInput2.text = string.Empty;
        }
        /// <summary>
        ///  注册
        /// </summary>
        private void Register()
        {
            //播放音效
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            // 密码和确认密码是否一致
            if ( pwdInput.text != pwdInput2.text )
            {
                Debug.LogError( "密码和确认密码必须一致!" );
                Alert.Show( "注册错误" , "密码和确认密码必须一致!" );
                return;
            }
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
            if ( !StringHelper.IsSafeSqlString( userInput.text )
                || StringHelper.CheckBadWord( userInput.text )
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
            user.uid = Guid.NewGuid().ToString( "N" );
            ReflashPanel();
            
            // 发送 注册 消息
            SendNotification( NotiList.REGISTER , user );
        }
    }
}
