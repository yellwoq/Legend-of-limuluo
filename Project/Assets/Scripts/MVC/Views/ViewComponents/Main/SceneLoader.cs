using Common;
using Components;
using SaveSystem;
using System.Collections;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace MVC
{
    /// <summary>
    ///  场景加载器
    /// </summary>
    public class SceneLoader : MonoSingleton<SceneLoader>
    {
        [SerializeField]
        private Loading currentLoading = null;
        [DisplayName("玩家交互界面画布")]
        public GameObject heroCanvas;
        [DisplayName("游戏操作界面画布")]
        public GameObject gameCanvas;
        public AsyncOperation ao;
        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="name">场景名</param>
        public void LoadScene(string name)
        {
            // 显示进度条
            currentLoading.Show();
            StartCoroutine(LoadSceneAsync(name));
        }
        IEnumerator LoadSceneAsync(string sceneName)
        {
            StartCoroutine(UPDateProgressSlider(sceneName));
            yield return new WaitUntil(() => ao.isDone);
            UIManager.I.TogglePanel(Panels.MainPanel, false);
            GameManager.I.Init();
            SendNotification(NotiList.GET_ITEM_MAP);
            Sound.SoundManager.I.PlayBgm("BGM2");
            Loading.I.Hide();
        }
        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="name"></param>
        public void ReturnMainScene()
        {
            // 显示进度条
            currentLoading.Show();
            StartCoroutine(ReturnMainSceneAsync());
        }
        IEnumerator ReturnMainSceneAsync()
        {
            StartCoroutine(UPDateProgressSlider("MainScene"));
            yield return new WaitUntil(() => ao.isDone);
            UIManager.I.TogglePanel(Panels.MainPanel, true);
            FindObjectOfType<ScenePanel>(true).isPause = false;
            FindObjectOfType<ScenePanel>(true).JudgePanelState();
            heroCanvas.GetComponent<CanvasGroup>().alpha = 0;
            heroCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            gameCanvas.GetComponent<CanvasGroup>().alpha = 0;
            heroCanvas.GetComponent<CanvasGroup>().blocksRaycasts = false;
            Sound.SoundManager.I.PlayBgm("Title");
            Loading.I.Hide();
        }
        //更新进度条
        public IEnumerator UPDateProgressSlider(string sceneName)
        {

            ao = SceneManager.LoadSceneAsync(sceneName);
            int displayProgress = 0;
            int toProgress = 0;
            Loading.I.SetSliderMax(100);
            ao.allowSceneActivation = false;
            while (ao.progress < 0.9f)
            {
                toProgress = 90;
                while (displayProgress < toProgress)
                {
                    displayProgress++;
                    Loading.I.UpdateProgress(displayProgress);
                    yield return new WaitForEndOfFrame();
                }
            }
            toProgress = 100;
            while (displayProgress < toProgress)
            {
                ++displayProgress;
                Loading.I.UpdateProgress(displayProgress);
                yield return new WaitForEndOfFrame();
            }
            ao.allowSceneActivation = true;
        }
    }
}
