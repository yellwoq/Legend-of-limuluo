using Common;
using Components;
using SaveSystem;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    /// 开始游戏面板
    /// </summary>
    public class StartPanel : BasePanel
    {
        /// <summary>
        /// 相关显示信息
        /// </summary>
        [HideInInspector]
        public Text heroName_txt, heroType_txt, herolv_txt;
        //格子的数据物体集合
        private LoopDataItem[] loopDataItem;
        //滚动条
        private LoopScrollView storeDataScrollView;
        //覆盖按钮
        private Button checkButton;
        //进度相关信息
        //用户id,英雄id,角色名
        private string userId, heroId, heroName, heroType;
        //角色等级
        private int lv;
        //角色数据存储文件名
        private string fileName;
        [HideInInspector]
        public bool hasRead = false;
        protected override void Awake()
        {
            base.Awake();
            storeDataScrollView = transform.FindChildComponentByName<LoopScrollView>("storeDataScrollView");
            Transform userBoderTF = GameObject.Find("userBoder").transform;
            heroName_txt = userBoderTF.FindChildByName("heroNameTitle").GetComponent<Text>();
            heroType_txt = userBoderTF.FindChildByName("heroTypeTitle").GetComponent<Text>();
            herolv_txt = userBoderTF.FindChildByName("heroLvTitle").GetComponent<Text>();
            checkButton = transform.FindChildComponentByName<Button>("checkButton");
            checkButton.onClick.AddListener(OverlayprogressClick);
        }
     
        /// <summary>
        /// 覆盖进度
        /// </summary>
        private void OverlayprogressClick()
        {
            string data = GameController.instance.crtUser.uid + "." + PlayerPrefs.GetString(KeyList.CURRENT_HERO_ID);
            Alert.Show("覆盖进度", "你确定要覆盖进度吗？", TryOverlay, data, true);
        }
        /// <summary>
        /// 尝试覆盖进度
        /// </summary>
        /// <param name="data">要覆盖的进度</param>
        private void TryOverlay(object data)
        {
            //发送删除进度信息
            SendNotification(NotiList.DELETE + NotiList.USER_HERO_DATA, data);
        }
        /// <summary>
        /// 显示相应的面板
        /// </summary>
        /// <param name="panel">相应的面板</param>
        public void ShowCreate(object panel)
        {
            Hide();
            UIManager.I.TogglePanel(panel.ToString(), true);
        }
        public override void Show()
        {
            base.Show();
            heroName_txt.gameObject.SetActive(true);
            heroType_txt.gameObject.SetActive(true);
            herolv_txt.gameObject.SetActive(true);
            StartCoroutine(InitData());
        }

        IEnumerator InitData()
        {
            //发送获取用户进度列表
            SendNotification(NotiList.GET_USER_HERO_LIST);
            yield return new WaitUntil(() => hasRead == true);
            hasRead = false;
        }
        public override void Hide()
        {
            base.Hide();
            heroName_txt.gameObject.SetActive(false);
            heroType_txt.gameObject.SetActive(false);
            herolv_txt.gameObject.SetActive(false);
            heroName_txt.text = "英雄名：";
            heroType_txt.text = "英雄类型：";
            herolv_txt.text = "英雄等级：";
        }
        /// <summary>
        /// 更新进度列表
        /// </summary>
        /// <param name="list">进度集合</param>
        public bool UpdateHeroList(List<UserHeroVO> list)
        {
            loopDataItem = new LoopDataItem[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                //读取到的相关信息赋值;
                userId = GameController.instance.crtUser.uid;
                heroId = list[i].heroId;
                heroName = list[i].heroName;
                heroType = list[i].heroType;
                lv = list[i].lv;

                fileName = list[i].fileName;

                string storeDir = Application.persistentDataPath + "/" + userId + "/" + heroId;
                //进度文件存储地址
                string storePath = storeDir + "/" + fileName + ".json";
                //如果玩家没有保存数据
                if (!File.Exists(storePath) || !File.Exists(storeDir + "/" + fileName + ".zdat"))
                {
                    GameController.I.crtHero.heroId = heroId;
                    string data = userId + "." + heroId;
                    SaveManager.I.heroDataName = fileName + ".json";
                    SaveManager.I.exceptHeroDataName = fileName + ".zdat";
                    new HeroProxy().DeleteHero(data, DeleteType.Auto);
                    continue;
                }
                HeroStateData heroData = SaveManager.I.LoadHeroData(storePath);
                //给每一格的数据赋值
                loopDataItem[i] = new LoopDataItem(i + 1, heroId, heroName, heroType, lv, heroData.currentMainQuestTitle, heroData.saveDate);
                loopDataItem[i].currentHeroData = list[i];
            }
            storeDataScrollView.InitData(loopDataItem);
            return true;
        }
    }
}
