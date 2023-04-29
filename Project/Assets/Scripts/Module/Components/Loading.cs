using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;

namespace Components
{
    /// <summary>
    ///  全局的进度条
    /// </summary>
    public class Loading : MonoSingleton<Loading>,IView
    {
        [DisplayName("进度条"),SerializeField]
        private Slider slider=null;
        [DisplayName("进度百分比显示文本"),SerializeField]
        private Text percentTxt=null;
        [SerializeField]
        private CanvasGroup mGroup=null;
        /// <summary>
        ///  显示
        /// </summary>
        public void Show()
        {
            //this.gameObject.SetActive( true );
            mGroup.alpha = 1;
            mGroup.blocksRaycasts = true;
        }
        /// <summary>
        /// 隐藏
        /// </summary>
        public void Hide()
        {
            // this.gameObject.SetActive( false );
            mGroup.alpha = 0;
            mGroup.blocksRaycasts = false;
        }
        /// <summary>
        ///  更新进度
        /// </summary>
        /// <param name="v"></param>
        public void UpdateProgress( float v )
        {
            slider.value = v;
            percentTxt.text = "正在加载：" + v+ "%";
        }
       
        public void SetSliderMax(float value)
        {
            slider.maxValue = value;
        }
    }
}
