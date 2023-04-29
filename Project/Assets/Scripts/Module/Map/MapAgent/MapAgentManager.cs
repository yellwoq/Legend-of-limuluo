using Common;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MapSystem
{
    /// <summary>
    /// 地图管理器，负责地图的启用或禁用
    /// </summary>
    public class MapAgentManager : MonoSingleton<MapAgentManager>
    {
        /// <summary>
        /// 所有的地图信息
        /// </summary>
        public List<MapAgent> MapList;
        [DisplayName("地图提醒文本")]
        public Text mapTipText;
        /// <summary>
        /// 初始化地图信息
        /// </summary>
        public void InitMap()
        {
            MapList.Clear();
            MapList.AddRange(FindObjectsOfType<MapAgent>(true));
        }
        /// <summary>
        /// 设置地图状态
        /// </summary>
        /// <param name="map"></param>
        /// <param name="isActive"></param>
        public void SetMap(string mapID, bool isActive)
        {
            MapList.Find(e => { return e.MapID == mapID; }).gameObject.SetActive(isActive);
            MapList.Find(e => { return e.MapID == mapID; }).SetPeople(isActive);
            if (isActive)
            {
                mapTipText.text = MapList.Find(e => { return e.MapID == mapID; }).MapName;
            }
        }
    }
}
