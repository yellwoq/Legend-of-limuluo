using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System;
using Common;

namespace MVC
{
    /// <summary>
    ///  加载语言列表文件
    /// </summary>
    public class LocaleDataLoader : MonoBehaviour
    {
        private void Awake()
        {
            // 加载XML
            LoadXML();
            // 初始化组件
            InitControlls();
            // 加载设置
            LoadSetting();
        }
        /// <summary>
        ///  加载设置
        /// </summary>
        private void LoadSetting()
        {
            int index = 0; // 默认索引
            if ( PlayerPrefs.HasKey( KeyList.LOCALE_INDEX ) )
                index = PlayerPrefs.GetInt( KeyList.LOCALE_INDEX );
            dropdown.value = index; // 设置索引
            dropdown.RefreshShownValue(); // 强制刷新
        }

        private Dropdown dropdown;
        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitControlls()
        {
            dropdown = GetComponent<Dropdown>();
            dropdown.options = options;
            dropdown.onValueChanged.AddListener( SaveSetting );
        }
        /// <summary>
        ///  保存设置
        /// </summary>
        /// <param name="arg0"></param>
        private void SaveSetting( int index )
        {
            // 保存索引
            PlayerPrefs.SetInt( KeyList.LOCALE_INDEX , index );
            string locale = options[ index ].text;
            // 保存语言
            PlayerPrefs.SetString( KeyList.LOCALE , locale );
        }

        /// <summary>
        ///  文件名
        /// </summary>
        private string fileName = "languages.xml";
        /// <summary>
        ///  文件路径
        /// </summary>
        private string FilePath
        {
            get { return Application.streamingAssetsPath + "/" + fileName; }
        }

        private List<Dropdown.OptionData> options;
        /// <summary>
        /// 加载XML
        /// </summary>
        private void LoadXML()
        {
            string xml = ConfigurationReader.GetConfigFile( fileName );
            Debug.Log( xml );
            // xmlDoc
            XmlDocument xmlDoc = new XmlDocument();
            // 加载文件
            //xmlDoc.Load( FilePath );
            xmlDoc.LoadXml( xml );
            Debug.Log( xmlDoc.InnerXml );
            // root
            XmlNode root = xmlDoc.SelectSingleNode( "root" );
            // 找到所有子节点
            XmlNodeList list = root.ChildNodes;
            // 初始化options
            options = new List<Dropdown.OptionData>();
            // 遍历
            foreach ( XmlElement e in list )
            {
                string s = e.GetAttribute( "name" );// name
                Debug.Log( s );
                options.Add( new Dropdown.OptionData( s ) );
            }
            
        }
    }
}
