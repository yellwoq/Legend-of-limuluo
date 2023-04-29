using Common;
using MVC;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    /// <summary>
    /// 安卓平台退出游戏
    /// </summary>
    public class AndroidExitGame : MonoBehaviour
    {
        [SerializeField]
        private Text msgText=null;
        float fadingSpeed = 1;
        bool fading;
        float startFadingTimep;
        Color originalColor;
        Color transparentColor;

        void Start()
        {
            originalColor = msgText.color;
            transparentColor = originalColor;
            transparentColor.a = 0;
            msgText.text = "再次按下返回键退出游戏";
            msgText.color = transparentColor;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (startFadingTimep == 0)
                {
                    msgText.color = originalColor;
                    startFadingTimep = Time.time;
                    fading = true;
                }
                else
                {
                    AppFacade.I.Stop();
                    GameObject[] games = FindObjectsOfType<GameObject>();
                    foreach (var item in games)
                    {
                        Destroy(item);
                    }
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                }
            }
            if (fading)
            {
                msgText.color = Color.Lerp(originalColor, transparentColor, (Time.time - startFadingTimep) * fadingSpeed);
                if (msgText.color.a < 2.0 / 255)
                {
                    msgText.color = transparentColor;
                    startFadingTimep = 0;
                    fading = false;
                }
            }
        }
    }
}