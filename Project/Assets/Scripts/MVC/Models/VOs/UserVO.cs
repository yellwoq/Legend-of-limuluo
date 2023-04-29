using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    ///  用户数据结构
    /// </summary>
    [System.Serializable]
    public class UserVO
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string uid;
        /// <summary>
        /// 用户名
        /// </summary>
        public string username;
        /// <summary>
        /// 用户密码
        /// </summary>
        public string password;

        public UserVO( string username, string password)
        {
            this.username = username;
            this.password = password;
        }

        public UserVO() { }
        
    }
}
