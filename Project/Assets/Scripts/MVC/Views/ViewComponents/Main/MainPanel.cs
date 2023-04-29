using Common;
using System.Collections.Generic;
using System.Text;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    public class MainPanel : BasePanel
    {
        private Button continueButton;
        private Button startButton;
        private Button userSettingButton;
        private Text userNameText;
        [SerializeField]
        private Text introduceText;
        [SerializeField]
        private List<GameObject> windows = null;
        private new void Awake()
        {
            //相关按钮绑定
            continueButton = Find<Button>("continueButton");
            startButton = Find<Button>("startButton");
            userSettingButton = Find<Button>("userSettingButton");
            continueButton.onClick.AddListener(OpenContinueClick);
            startButton.onClick.AddListener(OpenStartClick);
            userSettingButton.onClick.AddListener(OpenUserSettingClick);
            userNameText = Find<Text>("nameTitle");
        }
        public override void Show()
        {
            userNameText.text += GameController.instance.crtUser.username;
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            foreach (var window in windows)
            {
                window.SetActive(false);
            }
            StringBuilder introName = new StringBuilder("Config/故事介绍.txt");
            if (PlayerPrefs.HasKey(KeyList.LOCALE))
            {
                introName.Clear();
                switch (PlayerPrefs.GetString(KeyList.LOCALE))
                {
                    case "English":
                        introName.Append("Config/StoryIntroduce.txt");
                        break;
                    default:
                        introName.Append("Config/故事介绍.txt");
                        break;
                }
            }
            introduceText.text = ConfigurationReader.GetConfigFile(introName.ToString());
        }
        public override void Hide()
        {
            userNameText.text = string.Empty;
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            foreach (var window in windows)
            {
                window.SetActive(false);
            }
        }
        /// <summary>
        /// 打开读取游戏界面
        /// </summary>
        private void OpenUserSettingClick()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            UIManager.I.TogglePanelInChild(name, "UserSetPanel", true);
        }
        /// <summary>
        /// 打开开始游戏界面
        /// </summary>
        private void OpenStartClick()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            UIManager.I.TogglePanelInChild(name, "StartPanel", true);
        }
        /// <summary>
        /// 打开读取进度界面
        /// </summary>
        private void OpenContinueClick()
        {
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            UIManager.I.TogglePanelInChild(name, "ProgressReadePanel", true);
        }

    }
}
