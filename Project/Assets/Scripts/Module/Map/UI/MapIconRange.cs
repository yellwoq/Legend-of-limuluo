using UnityEngine;
using UnityEngine.UI;

namespace MapSystem
{
    /// <summary>
    /// 绘制的地图圆形区域
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MapIconRange : MonoBehaviour
    {
        public new Transform transform { get; private set; }
        /// <summary>
        /// 图标范围图像
        /// </summary>
        private Image range;
        /// <summary>
        /// 图标颜色
        /// </summary>
        public Color Color
        {
            get => range.color;
            set => range.color = value;
        }

        public RectTransform rectTransform;

        private void Awake()
        {
            range = GetComponent<Image>();
            range.raycastTarget = false;
            rectTransform = range.rectTransform;
            transform = base.transform;
        }
    }
}
