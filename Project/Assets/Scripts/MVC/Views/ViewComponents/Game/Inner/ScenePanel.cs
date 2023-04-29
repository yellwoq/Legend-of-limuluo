using Common;
using MapSystem;
using Player;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    /// 游戏中显示的界面
    /// </summary>
    public class ScenePanel : MonoBehaviour
    {
        private Button stopButton;
        [HideInInspector]
        public Button mapButton;
        //是否暂停
        public bool isPause = false;
        [SerializeField]
        private GameObject mapTip;
        private GameObject gameCanvas;
        private GameObject heroCanvas;
        /// <summary>
        /// 英雄的相关界面
        /// </summary>
        private GameObject gamePanels;
        private void Awake()
        {
            stopButton = transform.FindChildComponentByName<Button>("StopButton");
            mapButton = transform.FindChildComponentByName<Button>("MapButton");
            stopButton.onClick.AddListener(PauseGameClick);
            mapButton.onClick.AddListener(OpenMapClick);
            gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas");
            heroCanvas = GameObject.FindGameObjectWithTag("HeroCanvas");
            gamePanels = heroCanvas.transform.FindChildByName("GamePanels").gameObject;
        }
        /// <summary>
        /// 打开小地图
        /// </summary>
        private void OpenMapClick()
        {
            MapManager.I.OpenWindow();
        }
        public void JudgePanelState()
        {
            Texture2D t2D = null;
            if (isPause)
            {
                Time.timeScale = 0;
                t2D = ResourceManager.Load<Texture2D>("GameRun");
                stopButton.GetComponent<Image>().overrideSprite =
                Sprite.Create(t2D, new Rect(0, 0, t2D.width, t2D.height), new Vector2(0.5f, 0.5f));
                gamePanels.GetComponent<CanvasGroup>().alpha = 1;
                gamePanels.GetComponent<CanvasGroup>().blocksRaycasts = true;
                if (PlayerManager.I.playerTrans != null)
                    PlayerManager.I.playerTrans.GetComponent<PlayerMove>().enabled = false;
                UIManager.I.TogglePanel(Panels.ShopPanel, false, heroCanvas.transform);
                MapManager.I.CloseWindow();
            }
            else
            {
                Time.timeScale = 1;
                t2D = ResourceManager.Load<Texture2D>("GameStop");
                stopButton.GetComponent<Image>().overrideSprite =
                    Sprite.Create(t2D, new Rect(0, 0, t2D.width, t2D.height), new Vector2(0.5f, 0.5f));
                //隐藏所有交互面板
                Panels[] panels = { Panels.HeroInfoPanel, Panels.BagPanel, Panels.SystemPanel, Panels.SkillPanel };
                UIManager.I.TogglePanels(panels, false, heroCanvas.transform);
                QuestSystem.QuestManager.I.CloseQuestWindow();
                gamePanels.GetComponent<CanvasGroup>().alpha = 0;
                gamePanels.GetComponent<CanvasGroup>().blocksRaycasts = false;
                if (PlayerManager.I.playerTrans != null)
                    PlayerManager.I.playerTrans.GetComponent<PlayerMove>().enabled = true;
            }
            gameCanvas.GetComponent<CanvasGroup>().blocksRaycasts = !isPause;
            gameCanvas.GetComponent<CanvasGroup>().alpha = isPause ? 0 : 1;
            mapButton.gameObject.SetActive(!isPause);
            mapTip.SetActive(!isPause);
        }
        /// <summary>
        /// 打开英雄相关信息
        /// </summary>
        private void PauseGameClick()
        {
            isPause = !isPause;
            JudgePanelState();
        }
    }
}
