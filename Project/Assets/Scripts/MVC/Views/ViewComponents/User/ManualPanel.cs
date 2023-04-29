using UI;
using Common;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Localizational;
using System.Text;

namespace MVC
{
    /// <summary>
    /// 操作说明界面
    /// </summary>
    public class ManualPanel : BasePanel, IPointerClickHandler
    {
        private TextMeshProUGUI contentTxt;
        /// <summary>
        /// 当前的渲染相机
        /// </summary>
        [SerializeField]
        private Camera currentCamera=null;

        protected override void Awake()
        {
            base.Awake();
           contentTxt = transform.FindChildComponentByName<TextMeshProUGUI>("Content");
           
        }
        public override void Show()
        {
            base.Show();
            LocaleManager.UpdateEvent += UpdateText;
            UpdateText();
            
        }
        private void UpdateText()
        {
            StringBuilder textName = new StringBuilder("Config/中文操作说明.txt");
            if (PlayerPrefs.HasKey(KeyList.LOCALE))
            {
                textName.Clear();
                switch (PlayerPrefs.GetString(KeyList.LOCALE))
                {
                    case "English":
                        textName.Append("Config/OperateIntroduce.txt");
                        break;
                    default:
                        textName.Append("Config/中文操作说明.txt");
                        break;
                }
            }
            contentTxt.text = ConfigurationReader.GetConfigFile(textName.ToString());
        }
        public override void Hide()
        {
            contentTxt.text = string.Empty;
            base.Hide();
        }
        /// <summary>
        /// 点击超链接跳转
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(contentTxt, pos, currentCamera); //--UI相机
            if (linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = contentTxt.textInfo.linkInfo[linkIndex];
                Application.OpenURL(linkInfo.GetLinkText());
            }
        }
    }
}
