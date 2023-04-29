using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{
    /// <summary>
    /// 管理滚动窗口数据
    /// </summary>
    public class DataApdater : ILoopDataAdpater
    {
        #region 字段

        // 保存的所有的数据
        public List<object> allData = new List<object>();
        // 当前显示的数据,链表
        public LinkedList<object> currentShowData = new LinkedList<object>();

        #endregion


        #region 方法
        /// <summary>
        /// 获得将要得到的头部数据
        /// </summary>
        /// <returns></returns>
        public object GetHeaderData()
        {
            // 判断总数据的数量
            if (allData.Count == 0)
            {

                return null;
            }
            // 特殊的情况
            if (currentShowData.Count == 0)
            {
                object header = allData[0];
                currentShowData.AddFirst(header);
                return header;
            }

            // 获取到当前显示数据的第一个数据的前一个
            object t = currentShowData.First.Value;
            int index = allData.IndexOf(t);
            if (index != 0)
            {
                object header = allData[index - 1];
                // 加到当前显示的数据里面
                currentShowData.AddFirst(header);
                return header;
            }

            return null;
        }
        /// <summary>
        /// 移除头部数据
        /// </summary>
        /// <returns></returns>
        public bool RemoveHeaderData()
        {

            //如果只显示一个
            if (currentShowData.Count <= 2)
            {
                return false;
            }
            // 移除 currentShowData 第一个数据
            currentShowData.RemoveFirst();
            return true;
        }
        /// <summary>
        /// 获得将要得到的尾部数据
        /// </summary>
        /// <returns></returns>
        public object GetLastData()
        {

            // 判断总数据的数量
            if (allData.Count == 0)
            {

                return null;
            }

            // 特殊的情况
            if (currentShowData.Count == 0)
            {
                object l = allData[0];
                currentShowData.AddLast(l);
                return l;
            }
            
            object last = currentShowData.Last.Value;
            int index = allData.IndexOf(last);
            // 获取当前显示的最后一个的下一个
            //如果不是最后一个
            if (index != allData.Count - 1)
            {
                object now_last = allData[index + 1];
                currentShowData.AddLast(now_last);
                return now_last;
            }

            return null;
        }
        /// <summary>
        /// 移除尾部数据
        /// </summary>
        /// <returns></returns>
        public bool RemoveLastData()
        {
            // 移除 currentShowData 最后一个 
            if (currentShowData.Count == 0 || currentShowData.Count == 1) { return false; }
            currentShowData.RemoveLast();
            return true;
        }

        #endregion

        #region 数据管理
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="t"></param>
        public void InitData(object[] t)
        {
            allData.Clear();
            currentShowData.Clear();

            allData.AddRange(t);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="t"></param>
        public void InitData(List<object> t)
        {
            InitData(t.ToArray());
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="t"></param>
        public void AddData(object[] t)
        {
            allData.AddRange(t);
        }
        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="t"></param>
        public void AddData(List<object> t)
        {
            AddData(t.ToArray());
        }



        #endregion





    }
}
