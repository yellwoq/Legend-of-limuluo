using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    ///  面板的枚举类
    /// </summary>
    public enum Panels
    {
        //-----用户界面-----------
        LoginPanel,
        RegisterPanel,
        SettingPanel,
        OperateSettingPanel,
        ManualPanel,
        //-----主界面------------
        MainPanel,
        HeroCreatePanel,
        //-----全局---------
        Map,
        HeroPanel,
        Store,
        //----游戏界面-----------
        HeroInfoPanel,
        BagPanel,
        SkillPanel,
        SystemPanel,
        ShopPanel
    }
}
