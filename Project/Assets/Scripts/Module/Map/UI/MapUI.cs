using UnityEngine;
using UnityEngine.UI;

namespace MapSystem
{
    /// <summary>
    /// 地图UI
    /// </summary>
    public class MapUI : MonoBehaviour
    {
        [DisplayName("地图窗口")]
        public CanvasGroup mapWindow;
        [DisplayName("地图窗口UI坐标")]
        public RectTransform mapWindowRect;
        [DisplayName("地图遮罩UI坐标")]
        public RectTransform mapMaskRect;
        [DisplayName("地图图标预制体")]
        public MapIcon iconPrefab;
        [DisplayName("地图图标预制体父物体UI坐标")]
        public RectTransform iconsParent;
        [DisplayName("圆形范围显示预制体")]
        public MapIconRange rangePrefab;
        [DisplayName("圆形范围显示预制体父物体UI坐标")]
        public RectTransform rangesParent;
        [DisplayName("所有父物体的父物体UI坐标")]
        public RectTransform mainParent;
        [DisplayName("所产生的标志父物体UI坐标")]
        public RectTransform marksParent;
        [DisplayName("所产生的目标父物体UI坐标")]
        public RectTransform objectivesParent;
        [DisplayName("所产生的任务父物体UI坐标")]
        public RectTransform questsParent;
        public RectTransform mapRect;
        [DisplayName("地图内容显示UI")]
        public RawImage mapImage;
        [DisplayName("地图切换按钮")]
        public Button @switch;
        [DisplayName("定位玩家按钮")]
        public Button locate;
        [DisplayName("关闭按钮")]
        public Button close;
        private void Awake()
        {
            @switch.onClick.AddListener(MapManager.Instance.SwitchMapMode);
            locate.onClick.AddListener(MapManager.Instance.LocatePlayer);
            close.onClick.AddListener(MapManager.Instance.CloseWindow);
        }


    }
}