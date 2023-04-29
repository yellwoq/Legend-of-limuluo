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
    public class ProgressReadePanel : BasePanel
    {
        /// <summary>
        /// 相关显示信息
        /// </summary>
        [HideInInspector]
        public Text heroName_txt, heroType_txt, herolv_txt;
        //滚动条
        private LoopScrollView scrollView;
        //格子的数据物体集合
        private LoopDataItem[] loopDataItem;
        //进度相关信息
        //用户id,英雄id,角色名
        private string userId, heroId, heroName, heroType;
        //角色等级
        private int lv;
        //角色数据存储文件名
        private string fileName;
        [HideInInspector]
        public bool hasRead=false;
        private Button deleteButton;
        private Button checkButton;
        protected override void Awake()
        {
            base.Awake();
            Transform userBoderTF = GameObject.Find("userBoder").transform;
            heroName_txt = TransformHelper.FindChildByName(userBoderTF, "heroNameTitle").GetComponent<Text>();
            heroType_txt = TransformHelper.FindChildByName(userBoderTF, "heroTypeTitle").GetComponent<Text>();
            herolv_txt = TransformHelper.FindChildByName(userBoderTF, "heroLvTitle").GetComponent<Text>();
            scrollView = transform.FindChildComponentByName<LoopScrollView>("storeDataScrollView");
            checkButton = transform.FindChildComponentByName<Button>("checkButton");
            deleteButton = transform.FindChildComponentByName<Button>("deleteButton");
            checkButton.onClick.AddListener(ReadProgress);
            deleteButton.onClick.AddListener(TryDeleteProgress);
        }
        /// <summary>
        /// 尝试删除进度
        /// </summary>
        private void TryDeleteProgress()
        {
            string data = GameController.instance.crtUser.uid + "." + PlayerPrefs.GetString(KeyList.CURRENT_HERO_ID);
            Alert.Show("删除进度", "你确定要删除进度吗？", DeleteProgress, data, true);
        }
        /// <summary>
        /// 删除进度
        /// </summary>
        /// <param name="data"></param>
        private void DeleteProgress(object data)
        {
            //发送删除进度信息
            SendNotification(NotiList.DELETE + NotiList.USER_HERO_DATA, data);
        }
        /// <summary>
        /// 完成删除
        /// </summary>
        /// <param name="data"></param>
        public void DeleteComplete(object data = null)
        {
            //发送获取用户进度列表
            SendNotification(NotiList.GET_USER_HERO_LIST);
        }
        /// <summary>
        /// 读取进度
        /// </summary>
        private void ReadProgress()
        {
            ToggleGroup toggleGroup = transform.FindChildComponentByName<ToggleGroup>("Content");
            Toggle[] toggles = toggleGroup.GetComponentsInChildren<Toggle>();
            bool isNotChoose = true;
            foreach (var tog in toggles)
            {
                if (tog.isOn)
                {
                    isNotChoose = false;
                    break;
                }
            }
            if (isNotChoose)
            {
                Alert.Show("读取失败", "请选择进度");
                return;
            }
            SaveManager.I.Load();
        }

        public override void Show()
        {
            base.Show();
            heroName_txt.gameObject.SetActive(true);
            heroType_txt.gameObject.SetActive(true);
            herolv_txt.gameObject.SetActive(true);
            //发送获取用户进度列表
            SendNotification(NotiList.GET_USER_HERO_LIST);
            StartCoroutine(InitData());
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
        IEnumerator InitData()
        {
            yield return new WaitUntil(() => hasRead == true);
            hasRead = false;
        }
        /// <summary>
        /// 更新进度列表
        /// </summary>
        /// <param name="list"></param>
        public bool UpdateHeroList(List<UserHeroVO> list)
        {
            loopDataItem = new LoopDataItem[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                //读取到的相关信息赋值
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
                    string data = userId + "." + heroId;
                    GameController.I.crtHero.heroId = heroId;
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
            scrollView.InitData(loopDataItem);
            return true;
        }
    }
}
