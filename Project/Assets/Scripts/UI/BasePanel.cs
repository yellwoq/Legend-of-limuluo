using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVC;

namespace UI
{
    /// <summary>
    ///  显示和隐藏功能的面板,closeButton
    /// </summary>
    public class BasePanel : BaseMono,Components.IView
    {
        /// <summary>
        /// 关闭按钮
        /// </summary>
        protected Button closeButton;
        protected virtual void Awake()
        {
            closeButton = Find<Button>( "closeButton" );
            if ( closeButton != null )
                closeButton.onClick.AddListener( Hide );
        }
        /// <summary>
        ///  显示
        /// </summary>
        public virtual void Show()
        {
            this.gameObject.SetActive( true );
        }
        /// <summary>
        ///  隐藏
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive( false );
        }
    }
}
