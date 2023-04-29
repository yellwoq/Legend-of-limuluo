using UnityEngine;
using System.Collections;
using UnityEngine.Events;

namespace MapSystem
{
    /// <summary>
    /// 地图图标生成器，用于生成地图图标
    /// </summary>
    [AddComponentMenu("ZetanStudio/地图图标生成器")]
    public class MapIconHolder : MonoBehaviour
    {
        /// <summary>
        /// 图像
        /// </summary>
        public Sprite icon;
        /// <summary>
        /// 图标大小
        /// </summary>
        public Vector2 iconSize = new Vector2(48, 48);
        /// <summary>
        /// 图像偏移
        /// </summary>
        public Vector2 offset;
        /// <summary>
        /// 是否在世界地图绘制
        /// </summary>
        public bool drawOnWorldMap = true;
        /// <summary>
        /// 是否在地图中保持
        /// </summary>
        public bool keepOnMap = true;
        /// <summary>
        /// 最大的有效距离
        /// </summary>
        [SerializeField, Tooltip("小于零时表示显示状态不受距离影响。")]
        private float maxValidDistance = -1;
        /// <summary>
        /// 距离的平方
        /// </summary>
        [HideInInspector]
        public float DistanceSqr { get; private set; }
        /// <summary>
        /// 是否强制隐藏
        /// </summary>
        public bool forceHided;
        /// <summary>
        /// 是否可移除
        /// </summary>
        public bool removeAble;
        /// <summary>
        /// 是否显示范围(画圆）
        /// </summary>
        public bool showRange;
        /// <summary>
        /// 所处的范围内的颜色
        /// </summary>
        public Color rangeColor = new Color(1, 1, 1, 0.5f);
        /// <summary>
        /// 范围大小
        /// </summary>
        public float rangeSize = 144;
        /// <summary>
        /// 地图类型
        /// </summary>
        public MapIconType iconType;
        /// <summary>
        /// 实例化的地图图标
        /// </summary>
        public MapIcon iconInstance;
        /// <summary>
        /// 是否绘制
        /// </summary>
        public bool gizmos = true;
        /// <summary>
        /// 是否自动隐藏
        /// </summary>
        public bool AutoHide => maxValidDistance > 0;
        /// <summary>
        /// 在地图上显示的文本信息
        /// </summary>
        public string textToDisplay;

        public MapIconEvents iconEvents;
        /// <summary>
        /// 在手指点击时的事件
        /// </summary>
        public UnityEvent OnFingerClick => iconEvents.onFingerClick;
        /// <summary>
        /// 在鼠标点击时的事件
        /// </summary>
        public UnityEvent OnMouseClick => iconEvents.onMouseClick;
        /// <summary>
        /// 在鼠标进入时的事件
        /// </summary>
        public UnityEvent OnMouseEnter => iconEvents.onMouseEnter;
        /// <summary>
        /// 在鼠标离开时的事件
        /// </summary>
        public UnityEvent OnMouseExit => iconEvents.onMouseExit;
        /// <summary>
        /// 设置图标的有效距离
        /// </summary>
        /// <param name="distance"></param>
        public void SetIconValidDistance(float distance)
        {
            maxValidDistance = distance;
            DistanceSqr = maxValidDistance * maxValidDistance;
        }
        /// <summary>
        /// 产生图标
        /// </summary>
        public void CreateIcon()
        {
            if (MapManager.Instance)
            {
                //如果已经有能实例的地图图标
                if (iconInstance && iconInstance.gameObject)
                {
                    MapManager.Instance.RemoveMapIcon(this, true);
                }
                MapManager.Instance.CreateMapIcon(this);
            }
        }
        /// <summary>
        /// 显示图标
        /// </summary>
        /// <param name="zoom">缩放大小</param>
        public void ShowIcon(float zoom)
        {
            if (forceHided) return;
            if (iconInstance)
            {
                iconInstance.Show(showRange);
                //如果有图标范围
                if (iconInstance.iconRange)
                    //如果显示图标范围
                    if (showRange)
                    {
                        if (iconInstance.iconRange)
                        {
                            if (iconInstance.iconRange.Color != rangeColor) iconInstance.iconRange.Color = rangeColor;
                            iconInstance.iconRange.rectTransform.sizeDelta = new Vector2(rangeSize * 2, rangeSize * 2) * zoom;
                        }
                    }
                    else ZetanUtility.SetActive(iconInstance.iconRange.gameObject, false);
            }
        }
        /// <summary>
        /// 隐藏图标
        /// </summary>
        public void HideIcon()
        {
            if (iconInstance) iconInstance.Hide();
        }

        readonly WaitForSeconds WaitForSeconds = new WaitForSeconds(0.5f);
        /// <summary>
        /// 更新图标
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateIcon()
        {
            while (true)
            {
                if (iconInstance)
                {
                    //如果图标图像不是本图像
                    if (iconInstance.iconImage.overrideSprite != icon) iconInstance.iconImage.overrideSprite = icon;
                    //如果大小与本图像不一致
                    if (iconInstance.iconImage.rectTransform.rect.size != iconSize) iconInstance.iconImage.rectTransform.sizeDelta = iconSize;
                    iconInstance.iconType = iconType;
                    yield return WaitForSeconds;
                }
                else yield return new WaitUntil(() => iconInstance);
            }
        }

        #region MonoBehaviour
        void Start()
        {
            CreateIcon();
        }

        private void Awake()
        {
            DistanceSqr = maxValidDistance * maxValidDistance;
            StartCoroutine(UpdateIcon());
        }

        private void OnDrawGizmosSelected()
        {
            //如果可以进行绘制并且有地图管理器实例不在运行状态
            if (gizmos && MapManager.Instance && !Application.isPlaying)
            {
                if (MapManager.Instance.MapMaskRect)
                {
                    var rect = ZetanUtility.GetScreenSpaceRect(MapManager.Instance.MapMaskRect);
                    Gizmos.DrawCube(MapManager.Instance.MapMaskRect.position, iconSize * rect.width / MapManager.Instance.MapMaskRect.rect.width);
                    if (showRange)
                        ZetanUtility.DrawGizmosCircle(MapManager.Instance.MapMaskRect.position, rangeSize * rect.width / MapManager.Instance.MapMaskRect.rect.width,
                            Vector3.forward, rangeColor);
                }
            }
        }

        private void OnDestroy()
        {
            if (MapManager.Instance) MapManager.Instance.RemoveMapIcon(this, true);
        }
        #endregion
    }
    /// <summary>
    /// 地图图标事件类
    /// </summary>
    [System.Serializable]
    public class MapIconEvents
    {
        public UnityEvent onFingerClick = new UnityEvent();
        public UnityEvent onMouseClick = new UnityEvent();

        public UnityEvent onMouseEnter = new UnityEvent();
        public UnityEvent onMouseExit = new UnityEvent();

        public void RemoveAllListner()
        {
            onFingerClick.RemoveAllListeners();
            onMouseClick.RemoveAllListeners();
            onMouseEnter.RemoveAllListeners();
            onMouseExit.RemoveAllListeners();
        }
    }
}