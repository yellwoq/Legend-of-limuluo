using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVC;

namespace Components
{
    /// <summary>
    ///  具有选中功能的按钮
    /// </summary>
    public class SelectableButton : BaseMono
    {
        /// <summary>
        /// 选中框
        /// </summary>
        private Image selectedBorder;
        private void Awake()
        {
            selectedBorder = Find<Image>( "selectedBorder" );
        }
        /// <summary>
        ///  选中状态
        /// </summary>
        public bool Selected
        {
            set { selectedBorder.gameObject.SetActive( value ); }
        }

    }
}
