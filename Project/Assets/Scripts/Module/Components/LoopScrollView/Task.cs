using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Components
{

    /// <summary>
    /// 调用组件测试
    /// </summary>
    public class Task : MonoBehaviour
    {
        public LoopScrollView scrollView;

        private bool isGetData = false;

        private void Start()
        {

            LoopDataItem[] loopDataItem = new LoopDataItem[100];
            for (int i = 0; i < loopDataItem.Length; i++)
            {
                loopDataItem[i] = new LoopDataItem(i);
            }

            scrollView.InitData(loopDataItem);
            scrollView.onMoveDataEnd += this.OnMoveDataEnd;
        }
        public void OnMoveDataEnd()
        {

            // 去获取数据
            Debug.Log(" 去获取数据! ");

            StartGetData();
        }
        public void StartGetData()
        {
            if (isGetData == false)
            {
                Invoke("OnGetDataSuccess", 2);
                isGetData = true;
            }

        }
        public void OnGetDataSuccess()
        {

            isGetData = false;

            LoopDataItem[] loopDataItem = new LoopDataItem[100];
            for (int i = 0; i < loopDataItem.Length; i++)
            {
                loopDataItem[i] = new LoopDataItem(i + 100);
            }
            scrollView.AddData(loopDataItem);
        }
    }
}
