using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    /// <summary>
    /// 用于设置格子数据
    /// </summary>
    public class SetItemData : MonoBehaviour, ISetLoopItemData
    {

        /// <summary>
        /// 设置数据类型
        /// </summary>
        /// <param name="chileItem">格子预制件</param>
        /// <param name="data">数据</param>
        public void SetData(GameObject chileItem, object data)
        {
            LoopDataItem loopDataItem = (LoopDataItem)data;
            chileItem.GetComponent<LoopItem>().dataItem = loopDataItem;
            //绑定相关的属性
            chileItem.transform.Find("id").GetComponent<Text>().text = loopDataItem.id.ToString();
            chileItem.transform.Find("currentTaskTitle").GetComponent<Text>().text = loopDataItem.currentMainQuestTitle.ToString();
            chileItem.transform.Find("loginTimeTitle").GetComponent<Text>().text = loopDataItem.saveTime.ToString();

        }
    }
}
