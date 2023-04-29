using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace MVC
{
    /// <summary>
    ///  显示面板的按钮
    /// </summary>
    public class ShowPanelButton : MonoBehaviour
    {
        // btn
        private Button btn;
        private void Awake()
        {
            btn = GetComponent<Button>();
            // onClick
            btn.onClick.AddListener( ShowPanel );
        }
        // Panels
        /// <summary>
        ///  要显示的面板
        /// </summary>
        public Panels p;
        /// <summary>
        ///  显示面板
        /// </summary>
        private void ShowPanel()
        {
            // 播放菜单点击音效
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            // UIManager TogglePanel
            UIManager.I.TogglePanel( p , true );
        }
    }
}
