using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TarenaMVC;
using Components;
using Common;
using UI;

namespace MVC
{
    /// <summary>
    ///  用户系统的中介者
    /// </summary>
    public class UserMediator : Mediator
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public new const string NAME = "UserMediator";

        public UserMediator()
        {
            this.MediatorName = NAME;
            // 获取UserProxy
            userProxy = AppFacade.I.RetrieveProxy( UserProxy.NAME ) as UserProxy;
        }

        private UserProxy userProxy;
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public override string[] ListNotificationInterests()
        {
            return new string[] {
                NotiList.LOGIN,// 登录
                NotiList.LOGIN + NotiList.SUCCESS, // 登录成功
                NotiList.LOGIN + NotiList.FAILURE, // 登录失败
                NotiList.REGISTER, // 注册
                NotiList.REGISTER + NotiList.SUCCESS, // 注册成功
                NotiList.REGISTER + NotiList.FAILURE, // 注册失败
                NotiList.SHOW + NotiList.REGISTER, // 显示注册面板
                NotiList.SHOW + NotiList.LOGIN, // 显示登录面板
                NotiList.LOGOUT, // 注销
                NotiList.CHANGE+NotiList.USER_DATA,//更新用户数据
                NotiList.CHANGE + NotiList.USER_DATA + NotiList.SUCCESS,//更新数据成功
                NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE,//更新数据失败
            };
        }
        /// <summary>
        ///  处理消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public override void HandleNotification( string name , object data )
        {
            switch ( name )
            {
                case NotiList.LOGIN:// 登录
                    userProxy.Login( data as UserVO );
                    break;
                case NotiList.LOGIN + NotiList.SUCCESS:  // 登录成功
                    // 切换到主界面
                    // 显示MainPanel
                    UIManager.I.TogglePanel(Panels.MainPanel, true);
                    // 隐藏UserObjects
                    UIManager.I.ToggleObjects(Common.Tags.UserObjects ,false);
                    // 自动获取当前用户英雄
                    //SendNotification( NotiList.GET_CURRENT_HERO );
                    break;
                case NotiList.LOGIN + NotiList.FAILURE: // 登录失败
                    // 弹出警告框
                    Debug.Log( "用户名或密码错误!" );
                    Alert.Show( "登录错误" , "用户名或密码错误!" );
                    break;
                case NotiList.REGISTER: // 注册
                    // 调用userProxy的注册方法
                    userProxy.Register( data as UserVO );
                    break;
                case NotiList.REGISTER + NotiList.SUCCESS: // 注册成功
                    Alert.Show( "注册成功" , "新用户: " + ( data as UserVO ).username + " 注册成功!" );
                    break;
                case NotiList.REGISTER + NotiList.FAILURE: // 注册失败
                    Alert.Show( "注册失败" , data.ToString() );
                    break;
                case NotiList.SHOW + NotiList.REGISTER:// 显示注册面板
                    // 隐藏登录面板
                    UIManager.I.TogglePanel( Panels.LoginPanel , false );
                    // 显示注册面板
                    UIManager.I.TogglePanel( Panels.RegisterPanel , true );
                    break;
                case NotiList.SHOW + NotiList.LOGIN: // 显示登录面板
                    // 显示登录面板
                     UIManager.I.TogglePanel( Panels.LoginPanel , true );
                    // 隐藏注册面板
                    UIManager.I.TogglePanel( Panels.RegisterPanel , false );
                    break;
                case NotiList.LOGOUT: // 注销
                    // 隐藏主城界面
                    UIManager.I.TogglePanel(Panels.MainPanel, false );
                    // 显示登录界面UserObjects
                    UIManager.I.ToggleObjects(Common.Tags.UserObjects, true);
                    UIManager.I.TogglePanelInChild("MainPanel", "UserSetPanel", false);
                    userProxy.Logout();
                    break;
                case NotiList.CHANGE + NotiList.USER_DATA://更新用户数据
                    userProxy.RenovateData(data as UserVO);
                    break;
                case NotiList.CHANGE + NotiList.USER_DATA + NotiList.SUCCESS://更新数据成功
                    Alert.Show("更新成功", data.ToString());
                    //更新主界面用户个性化设置
                    MainPanel mainPanel=UIManager.I.GetPanel<MainPanel>();
                    Text nameTitle=mainPanel.transform.FindChildByName("nameTitle").GetComponent<Text>();
                    Debug.Log(nameTitle.name);
                    nameTitle.text = "欢迎："+ GameController.instance.crtUser.username;
                    break;
                case NotiList.CHANGE + NotiList.USER_DATA + NotiList.FAILURE://更新数据失败
                    Alert.Show("更新失败", data.ToString());
                    break;
            }
        }
    }
}
