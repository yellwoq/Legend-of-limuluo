using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MapSystem
{
    /// <summary>
    /// 单个图标类
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class MapIcon : MonoBehaviour, IPointerClickHandler,
    IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler
    {
        public new Transform transform { get; private set; }
        /// <summary>
        /// 图像显示图标
        /// </summary>
        [HideInInspector]
        public Image iconImage;
        /// <summary>
        /// 图标范围（圆形范围）
        /// </summary>
        [HideInInspector]
        public MapIconRange iconRange;
        /// <summary>
        /// 图像画布组
        /// </summary>
        public CanvasGroup ImageCanvas { get; private set; }
        /// <summary>
        /// 图标类型
        /// </summary>
        [HideInInspector]
        public MapIconType iconType;

        private bool forceHided;
        /// <summary>
        /// 是否强制隐藏
        /// </summary>
        public bool ForceHided => holder ? holder.forceHided : forceHided;

        private bool removeAble;
        /// <summary>
        /// 是否可移除
        /// </summary>
        public bool RemoveAble
        {
            get
            {
                return holder ? holder.removeAble : removeAble;
            }

            set
            {
                removeAble = value;
            }
        }

        private string textToDisplay;
        /// <summary>
        /// 将会显示的文本信息
        /// </summary>
        public string TextToDisplay
        {
            get
            {
                return holder ? holder.textToDisplay : textToDisplay;
            }

            set
            {
                textToDisplay = value;
            }
        }
        /// <summary>
        /// 所属地图生成器
        /// </summary>
        public MapIconHolder holder;
        /// <summary>
        /// 显示图标
        /// </summary>
        /// <param name="showRange"></param>
        public void Show(bool showRange = false)
        {
            if (ForceHided) return;
            ZetanUtility.SetActive(iconImage.gameObject, true);
            if (iconRange) ZetanUtility.SetActive(iconRange.gameObject, showRange);
        }
        /// <summary>
        /// 隐藏图标
        /// </summary>
        public void Hide()
        {
            ZetanUtility.SetActive(iconImage.gameObject, false);
            if (iconRange) ZetanUtility.SetActive(iconRange.gameObject, false);
        }
        /// <summary>
        /// 回收图标
        /// </summary>
        public void Recycle()
        {
            if (holder)
            {
                holder.iconInstance = null;
                holder = null;
            }
            iconImage.raycastTarget = true;
            RemoveAble = true;
            //如果有展示信息文本
            if (!string.IsNullOrEmpty(TextToDisplay) && TipsManager.Instance) TipsManager.Instance.Hide();
            TextToDisplay = string.Empty;
            if (iconRange) ObjectPool.Put(iconRange.gameObject);
            iconRange = null;
            ObjectPool.Put(gameObject);
        }
        /// <summary>
        /// 右击移除图标
        /// </summary>
        private void OnRightClick()
        {
            if (MapManager.Instance) MapManager.Instance.RemoveMapIcon(this, false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //如果按下的时左键
            if (eventData.button == PointerEventData.InputButton.Left)
            {
#if UNITY_STANDALONE
                if (holder) holder.OnMouseClick?.Invoke();
#elif UNITY_ANDROID
            if (holder) holder.OnFingerClick?.Invoke();
            if (!string.IsNullOrEmpty(TextToDisplay)) TipsManager.Instance.ShowText(transform.position, TextToDisplay, 2);
#endif
            }
            if (eventData.button == PointerEventData.InputButton.Right) OnRightClick();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
#if UNITY_ANDROID
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (pressCoroutine != null) StopCoroutine(pressCoroutine);
            pressCoroutine = StartCoroutine(Press());
        }
#endif
        }

        public void OnPointerUp(PointerEventData eventData)
        {
#if UNITY_ANDROID
        if (pressCoroutine != null) StopCoroutine(pressCoroutine);
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if UNITY_STANDALONE
            if (holder) holder.OnMouseEnter?.Invoke();
            if (!string.IsNullOrEmpty(textToDisplay)) TipsManager.Instance.ShowText(transform.position, textToDisplay);
#endif
        }

        public void OnPointerExit(PointerEventData eventData)
        {
#if UNITY_ANDROID
        if (pressCoroutine != null) StopCoroutine(pressCoroutine);
#endif
#if UNITY_STANDALONE
            if (holder) holder.OnMouseExit?.Invoke();
            if (!string.IsNullOrEmpty(textToDisplay)) TipsManager.Instance.HideText();
#endif
        }

        private void Awake()
        {
            iconImage = GetComponent<Image>();
            ImageCanvas = iconImage.GetComponent<CanvasGroup>();
            if (!ImageCanvas) ImageCanvas = iconImage.gameObject.AddComponent<CanvasGroup>();
            transform = base.transform;
        }

#if UNITY_ANDROID
    readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
    Coroutine pressCoroutine;

    IEnumerator Press()
    {
        float touchTime = 0;
        bool isPress = true;
        while (isPress)
        {
            touchTime += Time.fixedDeltaTime;
            if (touchTime >= 0.5f)
            {
                OnRightClick();
                yield break;
            }
            yield return WaitForFixedUpdate;
        }
    }
#endif
    }
    public enum MapIconType
    {
        Normal,
        Main,
        Mark,
        Quest,
        Objective
    }
}