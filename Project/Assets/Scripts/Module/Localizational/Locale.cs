using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Localizational
{
    /// <summary>
    ///  语言文件
    /// </summary>
    public class Locale 
    {
        /// <summary>
        /// 文件名
        /// </summary>
        private string locale;
        public Locale( string locale )
        {
            this.locale = locale;
            map = new Dictionary<string , string>();
        }
        /// <summary>
        ///  初始化
        /// </summary>
        public void Init()
        {
            // 加载语言文件
            string path = "language/" + locale + ".lang";
            string content = ConfigurationReader.GetConfigFile( path );
            Debug.Log( "path: " + path );
            Debug.Log( "content: " + content );
            // 解析
            ConfigurationReader.ReadConfig( content , ParseLine );
            // 更新所有文本
            LocaleManager.UpdateAllText();
        }
        /// <summary>
        ///  保存文本数据
        /// </summary>
        private Dictionary<string , string> map;
        /// <summary>
        ///  临时数组
        /// </summary>
        private string[] tmp;
        /// <summary>
        ///  解析行
        /// </summary>
        /// <param name="s">一行的内容</param>
        private void ParseLine( string s )
        {
            // 过滤空行和注释行
            if( s != string.Empty && s.IndexOf("#") != 0 )
            {
                tmp = s.Split( '=' );
                if ( tmp.Length == 2 )
                    map.Add( tmp[ 0 ] , tmp[ 1 ] );
            }
        }

        /// <summary>
        ///  获取文本
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns></returns>
        public string GetValue( string key )
        {
            return map[ key ];
        }

    }
}
