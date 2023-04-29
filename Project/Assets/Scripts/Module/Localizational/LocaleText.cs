using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Localizational
{
    /// <summary>
    ///  显示多语言文本
    /// </summary>
    [RequireComponent(typeof(Text))]
    public class LocaleText : MonoBehaviour
    {
        private Text localeText;
        private void Awake()
        {
            localeText = GetComponent<Text>();
        }
        /// <summary>
        ///  文本的key
        /// </summary>
        public string key;
        /// <summary>
        ///  更新文本
        /// </summary>
        private void UpdateText()
        {
            localeText.text = LocaleManager.I.GetValue( key );
        }
        /// <summary>
        ///  启用时更新文本
        /// </summary>
        private void OnEnable()
        {
            LocaleManager.UpdateEvent += UpdateText;
            UpdateText();
        }

        private void OnDisable()
        {
            LocaleManager.UpdateEvent -= UpdateText;
        }
    }
}
