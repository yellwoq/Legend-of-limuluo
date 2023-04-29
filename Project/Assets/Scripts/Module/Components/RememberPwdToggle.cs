using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVC;

namespace Components
{
    /// <summary>
    ///  记住密码开关
    /// </summary>
    [RequireComponent(typeof(Toggle))]
    public class RememberPwdToggle : MonoBehaviour
    {
        private Toggle pwdToggle;
        private void Awake()
        {
            pwdToggle = GetComponent<Toggle>();
            pwdToggle.onValueChanged.AddListener( TogglePwd );
            // 记住密码选中状态
            bool isOn = PlayerPrefs.HasKey( KeyList.REMEMBER_PWD )
                && PlayerPrefs.GetString( KeyList.REMEMBER_PWD ) == "true";
            pwdToggle.isOn = isOn;
        }
        /// <summary>
        ///  切换是否记住密码
        /// </summary>
        /// <param name="arg0"></param>
        private void TogglePwd( bool isOn )
        {
            // 保持是否记住密码
            PlayerPrefs.SetString( KeyList.REMEMBER_PWD , isOn ? "true" : "false" );
            // 不记住密码,删除记住的内容
            if ( !isOn )
            {
                PlayerPrefs.DeleteKey( KeyList.USERNAME );
                PlayerPrefs.DeleteKey( KeyList.PWD );
            }
        }
    }
}
