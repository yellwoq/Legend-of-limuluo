using MVC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TarenaMVC;
using Common;

namespace Bag
{
    /// <summary>
    /// 物品信息管理
    /// </summary>
    public class ItemInfoManager : MonoSingleton<ItemInfoManager>
    {
        //创建一个字典
        public Dictionary<int, BagItemVO> objectInfoDict;
        protected override void Initialize()
        {
            SendNotification(NotiList.GET_ITEM_MAP);
        }
        /// <summary>
        /// 通过Id从字典里查找到信息
        /// </summary>
        /// <param name="id">要查找的id</param>
        /// <returns></returns>
        public BagItemVO GetObjectInfoById(int id)
        {
            BagItemVO info = null;
            objectInfoDict.TryGetValue(id, out info);
            return info;
        }

        public GameItem GetGameItemById(int id)
        {
            BagItemVO bag = GetObjectInfoById(id);
            GameItem gameItem = new GameItem(id.ToString());
            return gameItem;
        }
        /// <summary>
        /// 更新背包信息
        /// </summary>
        /// <param name="objectMap"></param>
        public void UpDateMap(Dictionary<int, BagItemVO> objectMap)
        {
            objectInfoDict = objectMap;
            foreach (var item in objectInfoDict)
            {
                Debug.Log(item.Key + "----" + item.Value.name+",类型："+item.Value.bigType+"-"+item.Value.type);
            }
        }
    }
    /// <summary>
    /// 物品大类
    /// </summary>
    public enum BigItemType
    {
        Consumables,
        Equipment,
        MissionProp
    }
    /// <summary>
    /// 物品具体类别
    /// </summary>
    public enum DetailItemType
    {
        HpDrug,
        MpDrug,
        Weapon,
        Clothes,
        Shoes,
        MissionProp
    }
    /// <summary>
    /// 可应用的英雄类型
    /// </summary>
    public enum ApplyHeroType
    {
        Warrior=1,
        Wizard,
        Both
    }
}