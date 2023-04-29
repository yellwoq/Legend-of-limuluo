using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    ///  键名列表
    /// </summary>
    public class KeyList 
    {
        // 登陆面板
        public const string REMEMBER_PWD = "RememberPwd";//记住密码
        public const string USERNAME = "Username";//用户名
        public const string PWD = "Pwd";//密码      
        // 设置面板
        public const string LOCALE_INDEX = "LocaleIndex";//语言文件索引
        public const string LOCALE = "Locale";//语言
        // 进度面板
        public const string CURRENT_HERO_ID = "CurrentHeroID";//当前选择的英雄id
        //背包面板
        public const string CURRENT_ITEM_ID = "CurrentItemID";//当前物品id
        //商店
        public const string CURRENT_STOREITEM_ID = "CurrentStoreItemID";//当前商品物品id
        //技能面板
        public const string SKILL1 = "skill1";//技能1
        public const string SKILL2 = "skill2";//技能2
        public const string SKILL3 = "skill3";//技能3
    }
}
