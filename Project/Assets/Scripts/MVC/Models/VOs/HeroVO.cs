using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    ///  系统英雄数据结构
    /// </summary>
    public class HeroVO 
    {
        public string id; //英雄id
        public string type;   //英雄类型
        public string roleName; //类型名,
        public string description; //描述信息

        public override string ToString()
        {
            return "英雄id:" + id + "\ntype:" + type + "\n描述信息:" + description;
        }
    }
}
