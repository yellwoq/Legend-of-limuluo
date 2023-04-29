using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapSystem
{
    /// <summary>
    /// Ð¡µØÍ¼·¶Î§
    /// </summary>
    [Serializable, CreateAssetMenu(fileName = "mapArea", menuName = "RPG GAME/Map/new MapArea")]
    public class MinMapRange : ScriptableObject
    {
        [SerializeField, DisplayName("µØÍ¼ID")]
        private string mapID;

    }
}
