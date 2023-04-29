using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Sound
{
    /// <summary>
    ///  声音配置数据结构
    /// </summary>
    public class SettingVO 
    {
        /// <summary>
        ///  是否启用背景音乐
        /// </summary>
        public bool bgmEnabled = true;
        /// <summary>
        ///  背景音乐音量
        /// </summary>
        public float bgmVolume = 0.5f;
        /// <summary>
        ///  是否启用音效音乐
        /// </summary>
        public bool sfxEnabled = true;
        /// <summary>
        ///  音效音乐音量
        /// </summary>
        public float sfxVolume = 0.5f;

    }
}
