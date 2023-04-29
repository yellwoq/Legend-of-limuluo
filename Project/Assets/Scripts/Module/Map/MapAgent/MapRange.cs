using System;
using UnityEngine;

namespace MapSystem
{
    /// <summary>
    /// 地图范围
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "mapArea", menuName = "RPG GAME/Map/new MapArea")]
    public class MapRange : ScriptableObject
    {
        [SerializeField,DisplayName("地图ID")]
        private string mapID;
        public string MapID=>mapID;
        [SerializeField,DisplayName("跟随的最小范围")]
        private Vector2 rangeMin;
        public Vector2 RangeMin=>rangeMin;
        [SerializeField, DisplayName("跟随的最大范围")]
        private Vector2 rangeMax;
        public Vector2 RangeMax=>rangeMax;
    }
}
