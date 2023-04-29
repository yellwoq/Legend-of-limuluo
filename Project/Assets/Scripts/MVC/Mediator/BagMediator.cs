using Bag;
using Components;
using System.Collections;
using System.Collections.Generic;
using TarenaMVC;
using UnityEngine;

namespace MVC
{
    /// <summary>
    /// 背包、商城中介者
    /// </summary>
    public class BagMediator : Mediator
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public new const string NAME = "BagMediator";
        /// <summary>
        ///  构造函数
        /// </summary>
        public BagMediator()
        {
            this.MediatorName = NAME;
            // 获取heroProxy
            bagProxy = AppFacade.I.RetrieveProxy(BagProxy.NAME) as BagProxy;
        }
        /// <summary>
        /// BagProxy
        /// </summary>
        private BagProxy bagProxy;
        /// <summary>
        ///  监听的消息列表
        /// </summary>
        /// <returns></returns>
        public override string[] ListNotificationInterests()
        {
            return new string[] {
                NotiList.GET_ITEM_MAP,//获取物品列表
                NotiList.GET_ITEM_MAP + NotiList.SUCCESS,//获取物品列表成功
                NotiList.GET_ITEM_MAP+NotiList.FAILURE//获取物品列表失败
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
                case NotiList.GET_ITEM_MAP://获取物品表
                    bagProxy.GetItemMap();
                    break;
                case NotiList.GET_ITEM_MAP + NotiList.SUCCESS://获取物品表成功
                    ItemInfoManager.I.UpDateMap(data as Dictionary<int, BagItemVO>);
                    break;
                case NotiList.GET_ITEM_MAP + NotiList.FAILURE://获取物品表失败
                    Alert.Show("获取物品列表失败", data.ToString());
                    break;
            }
        }
    }
}
