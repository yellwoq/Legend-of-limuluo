using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Common;
using MVC;
using SaveSystem;

namespace Components
{
    /// <summary>
    /// 格子
    /// </summary>
    public class LoopItem : MonoBehaviour
    {

        #region 字段
        /// <summary>
        /// 当前物体的显示区域
        /// </summary>
        private RectTransform rect;
        /// <summary>
        /// 显示区域
        /// </summary>
        private RectTransform viewRect;
        /// <summary>
        /// 四个边的坐标
        /// </summary>
        private Vector3[] rectCorners;
        private Vector3[] viewCorners;
        /// <summary>
        /// 当前格子的数据
        /// </summary>
        public LoopDataItem dataItem;
        /// <summary>
        /// 滚动类型
        /// </summary>
        private LoopScrollViewType scrollViewType;

        /// <summary>
        /// 选择开关
        /// </summary>
        private Toggle selectTog;
        //相关显示信息
        Text heroName_txt, heroType_txt, herolv_txt;

        #endregion



        #region 事件

        public Action onAddHead;//增加头部
        public Action onRemoveHead;//回收头部
        public Action onAddLast;//增加尾部
        public Action onRemoveLast;//回收尾部

        #endregion

        private void Awake()
        {
            rect = transform.GetComponent<RectTransform>();
            //显示区域
            viewRect = transform.GetComponentInParent<ScrollRect>().GetComponent<RectTransform>();
            selectTog = transform.GetComponent<Toggle>();
            selectTog.onValueChanged.AddListener(OnSelectedClick);
            // 初始化数组
            rectCorners = new Vector3[4];
            viewCorners = new Vector3[4];
            ////相关信息初始化
            Transform userBoderTF = GameObject.Find("userBoder").transform;
            heroName_txt = userBoderTF.FindChildByName("heroNameTitle").GetComponent<Text>();
            heroType_txt = userBoderTF.FindChildByName("heroTypeTitle").GetComponent<Text>();
            herolv_txt = userBoderTF.FindChildByName("heroLvTitle").GetComponent<Text>();

        }

        void Update()
        {
            LinstenerCorners();
        }

        /// <summary>
        /// 监听格子状态
        /// </summary>
        public void LinstenerCorners()
        {
            // 获取自身的边界，边界的四个点，从左下角顺时针到右下角，世界坐标
            rect.GetWorldCorners(rectCorners);
            // 获取显示区域的边界
            viewRect.GetWorldCorners(viewCorners);
            //如果是头
            if (IsFirst())
            {
                switch (scrollViewType)
                {
                    case LoopScrollViewType.Horizontal:

                        if (rectCorners[3].x < viewCorners[0].x)
                        {
                            // 把头节点隐藏掉
                            onRemoveHead?.Invoke();
                        }

                        if (rectCorners[0].x > viewCorners[0].x)
                        {
                            // 增加一个头节点
                            if (onAddHead != null) { onAddHead(); }
                        }

                        break;
                    case LoopScrollViewType.Vertical:
                        //如果左下角已经超过了区域的上方范围
                        if (rectCorners[0].y > viewCorners[1].y)
                        {
                            // 把头节点给隐藏掉
                            if (onRemoveHead != null) { onRemoveHead(); }
                        }
                        //如果左上方超过了区域的下方范围
                        if (rectCorners[1].y < viewCorners[1].y)
                        {
                            // 添加头节点
                            onAddHead?.Invoke();
                        }
                        break;
                }


            }

            if (IsLast())
            {

                switch (scrollViewType)
                {
                    case LoopScrollViewType.Horizontal:

                        // 添加尾部
                        if (rectCorners[3].x < viewCorners[3].x)
                        {
                            if (onAddLast != null) { onAddLast(); }
                        }
                        // 回收尾部
                        if (rectCorners[0].x > viewCorners[3].x)
                        {
                            if (onRemoveLast != null) { onRemoveLast(); }
                        }

                        break;
                    case LoopScrollViewType.Vertical:
                        // 如果左下角的坐标大于区域的左下角坐标，添加尾部
                        if (rectCorners[0].y > viewCorners[0].y)
                        {
                            if (onAddLast != null) { onAddLast(); }
                        }
                        // 如果左上角的坐标大于区域的左下角坐标，回收尾部
                        if (rectCorners[1].y < viewCorners[0].y)
                        {
                            if (onRemoveLast != null) { onRemoveLast(); }
                        }
                        break;
                }


            }


        }
        /// <summary>
        /// 判断是不是头
        /// </summary>
        /// <returns></returns>
        public bool IsFirst()
        {
            //遍历所有的格子，直到找到一个是显示的
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                //如果自己是显示状态
                if (transform.parent.GetChild(i).gameObject.activeSelf)
                {
                    if (transform.parent.GetChild(i) == transform)
                    {
                        return true;
                    }
                    break;
                }
            }

            return false;
        }
        /// <summary>
        /// 判断是不是尾
        /// </summary>
        /// <returns></returns>
        public bool IsLast()
        {

            for (int i = transform.parent.childCount - 1; i >= 0; i--)
            {
                //如果最后一个是显示的
                if (transform.parent.GetChild(i).gameObject.activeSelf)
                {
                    //且当前是该物体
                    if (transform.parent.GetChild(i) == transform)
                    {
                        return true;
                    }
                    break;
                }
            }
            return false;
        }

        /// <summary>
        /// 设置LoopScrollView的类型
        /// </summary>
        /// <param name="loopScrollViewType">枚举类型</param>
        public void SetLoopScrollViewType(LoopScrollViewType loopScrollViewType)
        {
            scrollViewType = loopScrollViewType;
        }
        /// <summary>
        /// 更新头像框显示
        /// </summary>
        public void OnSelectedClick(bool isOn)
        {

            if (isOn)
            {
                PlayerPrefs.SetString(KeyList.CURRENT_HERO_ID, dataItem.heroID);
                heroName_txt.text = "英雄名：" + dataItem.heroName.ToString();
                heroType_txt.text = "英雄类型：" + dataItem.heroType.ToString();
                herolv_txt.text = "英雄等级：" + dataItem.herolv.ToString();
                GameController.I.crtHero = dataItem.currentHeroData;
                //数据相关文件保存
                SaveManager.I.heroDataName = GameController.I.crtHero.fileName + ".json";
                SaveManager.I.exceptHeroDataName = GameController.I.crtHero.fileName + ".zdat";
            }
        }
    }
}