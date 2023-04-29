using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Common;
using HedgehogTeam.EasyTouch;

namespace Bag
{
    /// <summary>
    /// 分页
    /// 计算背包页数
    //计算存储每一页horizontalNormalizedPosition的值
    //比较当前存储的值与释放（停止拖动）时的大小判断为左拖动还是右拖动
    //计算处与停止拖动时 horizontalNormalizedPosition距离最近的值，通过Update lerp取值，把horizontalNormalizedPosition设置为最近的值
    /// </summary>
    public class KnapsackPage : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public static KnapsackPage _instance;
        /// <summary>
        /// 滚动视窗
        /// </summary>
        public ScrollRect rect;
        /// <summary>
        /// 页数显示文本
        /// </summary>
        public Text pageInfo;
        /// <summary>
        /// 索引
        /// </summary>
        public float[] index;
        /// <summary>
        /// 速度
        /// </summary>
        public float smooth = 5.0f;
        /// <summary>
        /// 格子集合
        /// </summary>
        public Transform[] boxes;
      

        private RectTransform view, content;
        private bool isDrag;
        private int pageIndex = 0;
        public EasyTouch touch;
        void Awake()
        {
            _instance = this;
            
        }
        void Start()
        {
            //计算页数以及对应的 horizontalNormalizedPosition 值，当滚动位于最左边时horizontalNormalizedPosition == 0
            //，而位于最右边时为horizontalNormalizedPosition == 1。
            view = transform.Find("Viewport").GetComponent<RectTransform>();
            content = transform.FindChildComponentByName<RectTransform>("Content");
            boxes = new Transform[content.childCount];
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i] = content.GetChild(i);
            }
            //页数，24个为一页
            int pages = content.childCount / 24;
            float step = 1.0f / (pages - 1);
            index = new float[pages];
            index[0] = 0;
            for (int i = 1; i < pages; i++)
            {
                index[i] = index[i - 1] + step;
            }
            pageInfo.text = (pageIndex + 1).ToString() + "/" + pages.ToString();
            isDrag = false;
        }
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDrag = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //向左拉， pageindex增加
            if (index[pageIndex] > rect.horizontalNormalizedPosition)
            {
                pageIndex = pageIndex + 1 >= index.Length ? index.Length - 1 : pageIndex + 1;
            }
            else
            {
                pageIndex = pageIndex - 1 < 0 ? 0 : pageIndex - 1;
            }
            //计算释放时最近的页数对应的 horizontalNormalizedPosition
            float minDis = Mathf.Abs(index[pageIndex] - rect.horizontalNormalizedPosition);
            for (int i = 0; i < index.Length; i++)
            {
                if (minDis > Mathf.Abs(index[i] - rect.horizontalNormalizedPosition))
                {
                    minDis = Mathf.Abs(index[i] - rect.horizontalNormalizedPosition);
                    pageIndex = i;
                }
            }
            isDrag = false;
        }
        /// <summary>
        /// 左键点击时
        /// </summary>
        public void OnLeftBtnClicked()
        {
            pageIndex = pageIndex - 1 < 0 ? 0 : pageIndex - 1;
        }
        /// <summary>
        ///  右键点击时
        /// </summary>
        public void OnRightBtnClicked()
        {
            pageIndex = pageIndex + 1 >= index.Length ? index.Length - 1 : pageIndex + 1;
        }
        void Update()
        {
            //插值运算，达到缓动的目的
            if (isDrag == false && Mathf.Abs(rect.horizontalNormalizedPosition - index[pageIndex]) > 0.0015f)
            {
                rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, index[pageIndex],
                    Time.deltaTime * smooth);
                if (Mathf.Abs(rect.horizontalNormalizedPosition - index[pageIndex]) < 0.0015f)
                {
                    rect.horizontalNormalizedPosition = index[pageIndex];
                    pageInfo.text = (pageIndex + 1).ToString() + "/" + ((int)index.Length).ToString();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnLeftBtnClicked();
            }
            if (Input.GetMouseButtonDown(1))
            {
                OnRightBtnClicked();
            }
        }
    }
}
