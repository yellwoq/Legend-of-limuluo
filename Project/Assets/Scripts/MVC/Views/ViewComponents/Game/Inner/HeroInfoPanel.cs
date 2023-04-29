using Common;
using Player;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.UI;
namespace MVC
{
    /// <summary>
    /// 英雄信息显示面板
    /// </summary>
    public class HeroInfoPanel : BasePanel
    {
        ETCJoystick move;
        /// <summary>
        /// 相关的显示信息
        /// </summary>
        private Text nameInfo, lv, type, exp, money, force, spirit, intellect, speed, hp, mp, dem, def;
        private Slider hpSlider, mpSlider, expSlider;
        protected override void Awake()
        {
            nameInfo = transform.FindChildComponentByName<Text>("Name");
            lv = transform.FindChildComponentByName<Text>("Lv");
            type = transform.FindChildComponentByName<Text>("Type");
            exp = transform.FindChildComponentByName<Text>("Exp");
            money = transform.FindChildComponentByName<Text>("Money");
            force = transform.FindChildComponentByName<Text>("Force");
            spirit = transform.FindChildComponentByName<Text>("Spirit");
            intellect = transform.FindChildComponentByName<Text>("Intellect");
            speed = transform.FindChildComponentByName<Text>("Speed");
            hp = transform.FindChildComponentByName<Text>("Hp");
            mp = transform.FindChildComponentByName<Text>("Mp");
            dem = transform.FindChildComponentByName<Text>("Dem");
            def = transform.FindChildComponentByName<Text>("Def");
            hpSlider = transform.FindChildComponentByName<Slider>("HpSlider");
            mpSlider = transform.FindChildComponentByName<Slider>("MpSlider");
            expSlider = transform.FindChildComponentByName<Slider>("ExpSlider");
            move = FindObjectOfType<ETCJoystick>();
        }
        public override void Show()
        {
            base.Show();
            SetHeroData();
        }
        /// <summary>
        /// 设置英雄数据
        /// </summary>
        /// <returns></returns>
        public void SetHeroData()
        {
            UserHeroVO playerData =GameController.I.crtHero;
            Debug.Log(GameController.I.crtHero.DEM);
            nameInfo.text = playerData.heroName;
            lv.text = playerData.lv.ToString();
            type.text = playerData.heroType.ToString()== "Warrior" ? "战士" : "法师";
            money.text = playerData.money.ToString();
            force.text = playerData.force.ToString();
            spirit.text = playerData.spirit.ToString();
            intellect.text = playerData.intellect.ToString();
            speed.text = playerData.speed.ToString();
            hpSlider.maxValue = playerData.maxHP;
            mpSlider.maxValue = playerData.maxMP;
            expSlider.maxValue = playerData.nextLvNeedExp;
            hp.text = playerData.currentHP + "/" + playerData.maxHP.ToString();
            mp.text = playerData.currentMP + "/" + playerData.maxMP.ToString();
            exp.text = playerData.currentExp.ToString() + "/" + playerData.nextLvNeedExp;
            hpSlider.value = playerData.currentHP;
            mpSlider.value = playerData.currentMP;
            expSlider.value = playerData.currentExp;
            dem.text = FindObjectOfType<PlayerStatus>(true).DEM.ToString();
            def.text = FindObjectOfType<PlayerStatus>(true).DEF.ToString();
        }


    }
}
