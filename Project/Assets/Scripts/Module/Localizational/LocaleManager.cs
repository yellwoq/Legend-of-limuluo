using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using MVC;

namespace Localizational
{
    /// <summary>
    ///  语言管理器
    /// </summary>
    public class LocaleManager : MonoSingleton<LocaleManager>
    {
        /// <summary>
        ///  初始化
        /// </summary>
        protected override void Initialize()
        {
            // 初始化字典
            locales = new Dictionary<string , Locale>();
            // 加载保存的语言
            string locale = "简体中文";
            if ( PlayerPrefs.HasKey( KeyList.LOCALE ) )
                locale = PlayerPrefs.GetString( KeyList.LOCALE );
            ChangeLocale( locale );
        }
        // 当前语言
        private Locale crtLocale;
        // 所有语言对应表
        private Dictionary<string , Locale> locales;
        // 获取文本
        /// <summary>
        ///  获取当前语言对应的文本
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public string GetValue( string key )
        {
            return crtLocale.GetValue( key );
        }
        /// <summary>
        ///  切换语言
        /// </summary>
        /// <param name="localeName">切换的语言名</param>
        public void ChangeLocale( string localeName )
        {
            // locale有没有存过
            // 没有,则加载
            if( !locales.ContainsKey( localeName ) )
            {
                // 加载中文语言
                Locale locale = new Locale( localeName );
                crtLocale = locale;
                locale.Init();
                locales.Add( localeName , locale );
                return;
            }
            // 有缓存,直接切换
            crtLocale = locales[ localeName ];
            // 更新所有文本
            UpdateAllText();
        }
        // 更新所有文本

        public delegate void UpdateDelegate();// 定义委托
        public static UpdateDelegate UpdateEvent; // 委托对象
        /// <summary>
        ///  更新所有文本
        /// </summary>
        public static void UpdateAllText()
        {
            if( UpdateEvent != null )
            {
                Debug.Log( "更新所有文本!" );
                UpdateEvent();
            }
        }

    }
}
