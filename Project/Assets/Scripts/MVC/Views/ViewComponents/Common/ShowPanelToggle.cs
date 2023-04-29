using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    /// 显示面板的按钮
    /// </summary>
    public class ShowPanelToggle : MonoBehaviour
    {
        // btn
        private Toggle btn;
        private Transform canvas;
        private void Awake()
        {
            btn = GetComponent<Toggle>();
            // onClick
            btn.onValueChanged.AddListener(ShowPanel);
        }
        // Panels
        /// <summary>
        ///  要显示的面板
        /// </summary>
        public Panels p;
        /// <summary>
        ///  显示面板
        /// </summary>
        private void ShowPanel(bool isOn)
        {
            if (isOn)
            {
                // 播放菜单点击音效
                Sound.SoundManager.I.PlaySfx("ClickSfx");
                canvas = transform.GetComponentInParent<Canvas>().gameObject.transform;
                UIManager.I.TogglePanel(p, true,canvas);
                Panels[] panels = { Panels.HeroInfoPanel, Panels.BagPanel,Panels.SystemPanel,Panels.SkillPanel };
                foreach (var panel in panels)
                {
                    if (panel != p)
                        UIManager.I.TogglePanel(panel, false, canvas);
                }
                
            }
            else
            {
                UIManager.I.TogglePanel(p, false, canvas);
            }
        }
    }
}
