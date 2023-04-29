using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    ///  监听消息列表
    /// </summary>
    public class NotiList 
    {
        // 动作
        public const string SHOW = "Show"; // 显示
        public const string HIDE = "Hide"; // 隐藏
        public const string CHANGE = "Change";//更改
        public const string DELETE = "Delete";//删除
        // 结果
        public const string SUCCESS = "Success";//成功
        public const string FAILURE = "Failure";//失败
        // 用户系统
        public const string LOGIN = "Login";//登陆
        public const string REGISTER = "Register";//注册
        public const string LOGOUT = "Logout";//注销
        //主界面
        public const string USER_DATA = "UserData";//用户数据
        //创建英雄界面
        public const string GET_HERO_LIST = "GetHeroList"; // 获取系统英雄列表
        // 进度创建、进度存储读取界面
        public const string PLAY_LEVEL = "PlayLevel"; // 加载场景
        public const string GET_USER_HERO_LIST = "GetUserHeroList"; // 获取用户英雄列表
        public const string GET_CURRENT_HERO = "GetCurrentHero";// 获取当前英雄
        public const string USER_HERO_DATA = "UserHeroData"; // 用户英雄数据
        
        
        public const string SWITCH_HERO = "SwitchHero"; // 切换系统英雄
        public const string CREATE_HERO = "CreateHero"; // 创建新英雄
        public const string ADD_MONEY = "AddMoney"; // 增加金币
        public const string ADD_EXP = "AddExp";// 增加经验值
        // 主城场景
        public const string LEVEL_LOADED = "LevelLoaded";// 场景加载完毕
        public const string LOAD_MAINSCENE="LoadMainScene";//加载主界面
        // 背包、商城系统
        public const string GET_ITEM_MAP = "GetItemMap"; // 获取物品键值表
        public const string TRY_BUY_EQUIP = "TryBuyEquip"; // 尝试购买装备
        public const string BUY_EQUIP = "BuyEquip"; // 购买装备
    }
}
