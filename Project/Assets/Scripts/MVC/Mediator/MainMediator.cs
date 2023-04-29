using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TarenaMVC;
using UI;

namespace MVC
{
    /// <summary>
    ///  主界面的中介者：场景加载和卸载等
    /// </summary>
    public class MainMediator : Mediator
    {
        public new const string NAME = "MainMediator";

        public MainMediator()
        {
            this.MediatorName = NAME;
        }
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public override string[] ListNotificationInterests()
        {
            return new string[] {

                NotiList.PLAY_LEVEL, // 加载场景
                NotiList.LEVEL_LOADED, // 场景加载完毕
                NotiList.LOAD_MAINSCENE,//加载主界面
                
            };
        }
        /// <summary>
        ///  处理消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public override void HandleNotification( string name , object data )
        {
            if ( loader == null )
                loader = GameController.instance.GetComponent<SceneLoader>();

            switch ( name )
            {
                case NotiList.PLAY_LEVEL: // 加载场景
                    loader.LoadScene( data.ToString() );
                    break;
                case NotiList.LEVEL_LOADED: // 场景加载完毕
                    // 关闭Map
                    // 隐藏主面板MainPanel,英雄创造面板HeroCreatePanel
                    UIManager.I.ToggleObjects(Common.Tags.MainObjects, false);
                    UIManager.I.TogglePanel( Panels.MainPanel , false );
                    UIManager.I.TogglePanel(Panels.HeroCreatePanel, false);
                    break;
                case NotiList.LOAD_MAINSCENE://加载主界面
                    loader.ReturnMainScene();
                    break;
            }
        }

        private SceneLoader loader;

    }
}
