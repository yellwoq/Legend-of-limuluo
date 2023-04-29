using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MapSystem
{
    /// <summary>
    /// 地图
    /// </summary>
    public class MapAgent : MonoBehaviour
    {
        [DisplayName("地图ID"), SerializeField]
        private string mapID=null;
        public string MapID { get { return mapID; } }
        [SerializeField,DisplayName("地图名称")]
        private string mapName = null;
        public string MapName =>mapName;
        private Tilemap myTileMap;
        public Tilemap MyTileMap
        {
            get
            {
                return GetComponent<Tilemap>();
            }
        }
        [DisplayName("是否有人物")]
        public bool isHavePeople;
        [ConditionalReadOnly("相关的人物", "isHavePeople",true)]
        public List<GameObject> relatePeople;
        /// <summary>
        /// 设置状态
        /// </summary>
        public void SetState(bool state)
        {
            gameObject.SetActive(state);
        }
        /// <summary>
        /// 设置相关人物状态
        /// </summary>
        /// <param name="state"></param>
        public void SetPeople(bool state)
        {
            if (!isHavePeople) return;
            foreach (var go in relatePeople)
            {
                go.SetActive(state);
            }
        }
    }
}
