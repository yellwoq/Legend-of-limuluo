using Bag;
using Common;
using Components;
using Player;
using QuestSystem;
using UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MapSystem;
#pragma warning disable CS0649
namespace MVC
{
    /// <summary>
    /// 系统界面
    /// </summary>
    public class SystemPanel : BasePanel
    {
        [SerializeField]
        private Button saveBtn, helpBtn, mainBtn;
        [DisplayName("帮助窗口"), SerializeField]
        private GameObject helpWindow;
        protected override void Awake()
        {
            saveBtn.onClick.AddListener(SaveGame);
            helpBtn.onClick.AddListener(ShowHelpPanel);
            mainBtn.onClick.AddListener(ReturnMainScene);
        }

        public void SaveGame()
        {
            SaveSystem.SaveManager.I.Save();
            //保存玩家数据
            SaveSystem.SaveManager.I.SaveHeroData(PlayerManager.I.SetHeroSaveData());
        }

        public void ShowHelpPanel()
        {
            helpWindow.GetComponent<ManualPanel>().Show();
        }

        public void ReturnMainScene()
        {
            Alert.Show("返回主界面", "您确定要返回主界面吗？如果没有保存进度将会丢失", (e) =>{
                BagPanel.I.ClearData();
                MapManager.I.ClearAllMarks();
                GameObjectPool.I.ClearAll();
                Destroy(FindObjectOfType<PlayerStatus>(true));
                SendNotification(NotiList.LOAD_MAINSCENE);
            }, null, true);
        }
    }
}
