using Common;
using Components;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    /// 创建新英雄面板
    /// </summary>
    public class HeroCreatePanel : BasePanel
    {
        private Dropdown heroTypeDropdown;
        private Button startButton;
        private Text contentTxt;
        private InputField nameInputField;
        private Image weaponImg;
        private HeroType heroType; //英雄类型

        private Scrollbar scrollbar;
        [SerializeField, DisplayName("系统英雄")]
        private List<SystemHero> systemHeroes;
        /// <summary>
        ///所有英雄的相关数据
        /// </summary>
        public Dictionary<string, HeroVO> heroMap;

        /// <summary>
        /// 英雄类型选项
        /// </summary>
        private List<Dropdown.OptionData> options;
        [InspectorName("武器"),DisplayName("英雄武器展示")]
        public Sprite[] heroTypeWeapon;
        protected override void Awake()
        {
            base.Awake();
            heroTypeDropdown = Find<Dropdown>("heroTypeDrop");
            heroTypeDropdown.onValueChanged.AddListener(SwitchHero);
            startButton = Find<Button>("startButton");
            startButton.onClick.AddListener(StartGame);
            contentTxt = transform.FindChildComponentByName<Text>("contentTxt");
            nameInputField = transform.FindChildComponentByName<InputField>("nameInputField");
            weaponImg = Find<Image>("weaponImg");
            scrollbar = transform.FindChildComponentByName<Scrollbar>("Scrollbar Vertical");
        }

        private void SwitchHero(int myChooseIndex)
        {
            Switch((HeroType)myChooseIndex);
            scrollbar.value = 1;
            weaponImg.sprite = heroTypeWeapon[myChooseIndex];
        }

        public override void Show()
        {
            base.Show();
            InitData();
            scrollbar.value = 1;
            weaponImg.sprite = heroTypeWeapon[0];
            nameInputField.text = string.Empty;
        }

        public override void Hide()
        {
            base.Hide();
            nameInputField.text = string.Empty;
            //发送获取用户进度列表
            SendNotification(NotiList.GET_USER_HERO_LIST);
        }
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            //如果没有赋值
            if (systemHeroes == null)
            {
                return;
            }
            heroMap = new Dictionary<string, HeroVO>();
            for (int i = 0; i < systemHeroes.Count; i++)
            {
                HeroVO hero = new HeroVO();
                hero.id = systemHeroes[i].HeroID.ToString();
                hero.roleName = systemHeroes[i].RoleName;
                hero.type = systemHeroes[i].HeroType;
                hero.description = systemHeroes[i].Decription;
                heroMap.Add(hero.type, hero);
            }
            // 初始化options
            options = new List<Dropdown.OptionData>();
            foreach (var hero in heroMap)
            {
                Debug.Log(hero.Value.roleName);
                options.Add(new Dropdown.OptionData(hero.Value.roleName));
            }
            heroTypeDropdown.options = options;
            // 默认切换英雄
            Switch(HeroType.Warrior);
        }
        /// <summary>
        /// 开始游戏按钮
        /// </summary>
        private void StartGame()
        {
            //点击音效
            Sound.SoundManager.I.PlaySfx("ClickSfx");
            // 检查用户名和密码是否为空
            if (nameInputField.text == string.Empty)
            {
                Debug.LogError("英雄名不能为空!");
                // 弹出框
                Alert.Show("进入游戏失败", "英雄名不能为空!");
                return;
            }
            // 检查是否有非法字符
            if (!StringHelper.IsSafeSqlString(nameInputField.text)
                || StringHelper.CheckBadWord(nameInputField.text))
            {
                Debug.LogError("英雄名不能有非法字符!");
                // 弹出框
                Alert.Show("进入游戏失败", "英雄名不能有非法字符!");
                return;
            }
            StartCoroutine(StartCreatHero());
        }

        /// <summary>
        ///  切换英雄信息
        /// </summary>
        /// <param name="type"></param>
        public void Switch(HeroType type)
        {
            heroType = type;
            heroTypeDropdown.value = (int)type;
            contentTxt.text = heroMap[type.ToString()].description;
        }

        private IEnumerator StartCreatHero()
        {
            yield return new WaitForSeconds(0.5f);
            // UserHeroVO
            UserHeroVO hero = new UserHeroVO();
            hero.heroName = nameInputField.text; // name
            hero.heroType = heroType.ToString(); // type
            hero.userId = GameController.instance.crtUser.uid; // userId
            hero.heroId = Guid.NewGuid().ToString("N");// heroId
            hero.fileName = GameController.instance.crtUser.uid + "." +
            GameController.instance.crtUser.username + "." +
            hero.heroId + "." + hero.heroName;
            // 发送 创建新英雄 消息
            SendNotification(NotiList.CREATE_HERO, hero);
        }
    }
}
