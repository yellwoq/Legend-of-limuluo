using Components;
using Player;
using System.Collections.Generic;
using TarenaMVC;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace MVC
{
    /// <summary>
    ///  角色系统的中介者
    /// </summary>
    public class HeroMediator : Mediator
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public new const string NAME = "HeroMediator";
        /// <summary>
        ///  构造函数
        /// </summary>
        public HeroMediator()
        {
            this.MediatorName = NAME;
            // 获取heroProxy
            heroProxy = AppFacade.I.RetrieveProxy(HeroProxy.NAME) as HeroProxy;

        }
        /// <summary>
        /// HeroProxy
        /// </summary>
        private HeroProxy heroProxy;
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public override string[] ListNotificationInterests()
        {
            return new string[] {
                NotiList.GET_USER_HERO_LIST, // 获取用户英雄列表
                NotiList.GET_USER_HERO_LIST + NotiList.SUCCESS , // 获取用户英雄列表成功
                
                NotiList.GET_HERO_LIST, // 获取系统英雄列表
                NotiList.GET_HERO_LIST + NotiList.SUCCESS,  // 获取系统英雄列表 成功
                NotiList.SWITCH_HERO, // 切换系统英雄

                NotiList.CREATE_HERO, // 创建新英雄
                NotiList.CREATE_HERO + NotiList.SUCCESS, // 创建新英雄 成功
                NotiList.CREATE_HERO + NotiList.FAILURE, // 创建新英雄 失败

                NotiList.DELETE + NotiList.USER_HERO_DATA, // 删除进度
                NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.SUCCESS,//删除进度成功
                NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.FAILURE,//删除进度失败

                NotiList.GET_CURRENT_HERO, // 获取当前英雄
                NotiList.GET_CURRENT_HERO + NotiList.SUCCESS, // 获取当前英雄成功
                NotiList.GET_CURRENT_HERO+NotiList.FAILURE,//获取当前英雄失败
                NotiList.CHANGE + NotiList.USER_HERO_DATA,//更新英雄数据
                NotiList.CHANGE + NotiList.USER_HERO_DATA+NotiList.SUCCESS,//更新英雄数据成功
                NotiList.CHANGE + NotiList.USER_HERO_DATA+NotiList.FAILURE,//获取当前英雄失败
            };
        }
        /// <summary>
        ///  处理角色系统相关消息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        public override void HandleNotification(string name, object data)
        {
            switch (name)
            {
                case NotiList.GET_USER_HERO_LIST: // 获取用户英雄列表
                    // 调用heroProxy的GetUserHeroList方法
                    heroProxy.GetUserHeroList();
                    break;
                case NotiList.GET_USER_HERO_LIST + NotiList.SUCCESS: // 获取用户英雄列表成功
                    // 找到StartPanel,Progess
                    StartPanel startPanel = UIManager.I.GetPanelInChild<StartPanel>("MainPanel", "StartPanel");
                    ProgressReadePanel progressReadePanel = UIManager.I.GetPanelInChild<ProgressReadePanel>("MainPanel", "ProgressReadePanel");
                    // 数据给进度列表
                    if (startPanel.gameObject.activeSelf)
                        startPanel.hasRead= startPanel.UpdateHeroList(data as List<UserHeroVO>);
                    if (progressReadePanel.gameObject.activeSelf)
                    {
                        progressReadePanel.hasRead = progressReadePanel.UpdateHeroList(data as List<UserHeroVO>);
                    }
                    break;
                //case NotiList.GET_HERO_LIST: // 获取系统英雄列表
                //    heroProxy.GetHeroList();
                //    break;
                //case NotiList.GET_HERO_LIST + NotiList.SUCCESS:  // 获取系统英雄列表 成功
                //    //数据给HeroCreatePanel
                //    HeroCreatePanel heroCP = UIManager.I.GetPanel<HeroCreatePanel>();
                //    heroCP.heroMap = data as Dictionary<string, HeroVO>;
                //    //初始化数据
                //    heroCP.InitData();
                //    // 默认切换英雄
                //    heroCP.Switch(HeroType.Warrior);
                //    break;
                case NotiList.CREATE_HERO: // 创建新英雄
                    heroProxy.CreateHero(data as UserHeroVO);
                    break;
                case NotiList.CREATE_HERO + NotiList.SUCCESS: // 创建新英雄 成功
                    //开始游戏场景
                    SendNotification(NotiList.PLAY_LEVEL, "GameScene");
                    break;
                case NotiList.CREATE_HERO + NotiList.FAILURE: // 创建新英雄 失败
                    Alert.Show("新建英雄失败", data.ToString());
                    break;
                case NotiList.DELETE + NotiList.USER_HERO_DATA://删除用户英雄进度信息
                    heroProxy.DeleteHero(data.ToString());
                    break;
                case NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.SUCCESS:
                    // 找到StartPanel,Progess
                    startPanel = UIManager.I.GetPanelInChild<StartPanel>("MainPanel", "StartPanel");
                    progressReadePanel = UIManager.I.GetPanelInChild<ProgressReadePanel>("MainPanel", "ProgressReadePanel");
                    UnityAction<object> callback = null;
                    // 数据给进度列表
                    if (startPanel.gameObject.activeSelf)
                    {
                        callback = startPanel.ShowCreate;
                        data = "HeroCreatePanel";
                    }
                    if (progressReadePanel.gameObject.activeSelf)
                    {
                        callback = progressReadePanel.DeleteComplete;
                    }
                    Alert.Show("删除成功", "原进度已经被移除", callback, data);
                    break;
                case NotiList.DELETE + NotiList.USER_HERO_DATA + NotiList.FAILURE:
                    Alert.Show("删除失败", "原进度未被移除");
                    break;
                case NotiList.GET_CURRENT_HERO:// 获取当前英雄
                    heroProxy.GetCurrentHero();
                    break;
                case NotiList.GET_CURRENT_HERO + NotiList.SUCCESS://获取当前英雄成功
                    PlayerManager.I.heroData = new SaveSystem.HeroStateData(data as UserHeroVO);
                    break;
                case NotiList.GET_CURRENT_HERO + NotiList.FAILURE://获取当前英雄失败
                    Alert.Show("读取失败", data.ToString());
                    break;
                case NotiList.CHANGE + NotiList.USER_HERO_DATA://更新英雄数据
                    heroProxy.RenovateHero(data as UserHeroVO);
                    break;
                    case NotiList.CHANGE + NotiList.USER_HERO_DATA + NotiList.SUCCESS://更新英雄数据成功
                    PlayerManager.I.heroData.SetHeroSave(data as UserHeroVO);
                    break;
                case NotiList.CHANGE + NotiList.USER_HERO_DATA + NotiList.FAILURE://获取当前英雄失败
                    Alert.Show("操作失败", data.ToString());
                    break;
            }
        }
    }
}
