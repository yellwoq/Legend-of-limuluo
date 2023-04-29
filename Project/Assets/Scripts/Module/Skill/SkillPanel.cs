using Common;
using Components;
using MVC;
using Player;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace SkillSystem
{
    /// <summary>
    /// 技能面板，处理技能的升级与装备
    /// </summary>
    public class SkillPanel : BasePanel
    {
        // 所有技能
        [DisplayName("显示在面板的技能")]
        public SkillItem[] skillItemPreViews;
        [SerializeField, DisplayName("技能1", true)]
        private Skill skill1;
        [SerializeField, DisplayName("技能2", true)]
        private Skill skill2;
        [SerializeField, DisplayName("技能3", true)]
        private Skill skill3;
        [SerializeField, DisplayName("当前选中的技能", true)]
        private SkillItem crtItem;
        [SerializeField, DisplayName("当前技能点显示文本")]
        private Text skillPointTip;
        [SerializeField, DisplayName("技能描述窗口"), Header("技能描述相关")]
        private GameObject skillDescriptionWindow;
        [SerializeField, DisplayName("技能名称文本")]
        private Text skillName;
        [SerializeField, DisplayName("技能等级文本")]
        private Text skillLv;
        [SerializeField, DisplayName("魔法消耗文本")]
        private Text MpCons;
        [SerializeField, DisplayName("冷却时间文本")]
        private Text coolTime;
        [SerializeField, DisplayName("伤害文本")]
        private Text damage;
        [SerializeField, DisplayName("技能描述文本")]
        private Text descrip;
        [SerializeField, DisplayName("描述窗口关闭按钮")]
        private Button descloseButton;
        [SerializeField, DisplayName("技能升级按钮")]
        private Button levelUpButton;
        [SerializeField, DisplayName("技能配置窗口显示按钮")]
        private Button showConfigButton;
        [SerializeField, DisplayName("技能配置窗口"), Header("技能配置相关")]
        private GameObject skillConfigWindow;
        [SerializeField, DisplayName("当前技能信息显示文本")]
        private Text currentskillName;
        [SerializeField, DisplayName("确定按钮")]
        private Button checkButton;
        [SerializeField, DisplayName("配置窗口关闭按钮")]
        private Button configCloseButton;
        [SerializeField, DisplayName("技能配置按钮")]
        private PlayerInputButton[] btns;
        [SerializeField, DisplayName("是否保存", true)]
        private bool isSave = false;

        #region MonoBehaviour
        public override void Show()
        {
            skillPointTip.text = "当前技能点：" + SkillManager.I.skillpoint;
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            // 遍历skills
            for (int i = 1; i < SkillManager.I.allSkills.Count; i++)
            {
                SkillItem skillItem = skillItemPreViews.Find(e => { return e.name.Contains(i.ToString()); });
                Skill skillData = SkillManager.I.allSkills.Find(e => { return e.SkillID.Contains(i.ToString()); });
                // 设置Data
                skillItem.Data = skillData;
                // 处理点击
                skillItem.GetComponent<Button>().onClick.AddListener(delegate { Select(skillItem); });
            }
        }
        protected override void Awake() { }
        public override void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        #endregion

        #region 技能描述窗口相关
        /// <summary>
        /// 选择当前技能
        /// </summary>
        /// <param name="item"></param>
        private void Select(SkillItem item)
        {
            if (crtItem != null) crtItem.Selected = false;
            item.Selected = true;
            //先关闭之前面板
            CloseSkillConfigWindow();
            CloseSkillDesWindow();
            crtItem = item;
            // 打开技能详细面板
            OpenSkillDesWindow();
        }
        /// <summary>
        /// 打开技能详细面板
        /// </summary>
        private void OpenSkillDesWindow()
        {
            skillDescriptionWindow.SetActive(true);
            UpDateDescriptionWindow();
            //相关事件绑定
            descloseButton.onClick.AddListener(CloseSkillDesWindow);
            levelUpButton.onClick.AddListener(SkillLeveUp);
            showConfigButton.onClick.AddListener(OpenSkillConfigWindow);
        }
        /// <summary>
        /// 关闭技能详细面板
        /// </summary>
        private void CloseSkillDesWindow()
        {
            skillDescriptionWindow.SetActive(false);
            descloseButton.onClick.RemoveListener(CloseSkillDesWindow);
            levelUpButton.onClick.RemoveListener(SkillLeveUp);
            showConfigButton.onClick.RemoveListener(OpenSkillConfigWindow);
        }
        /// <summary>
        /// 更新描述窗口
        /// </summary>
        private void UpDateDescriptionWindow()
        {
            skillName.text = "技能名称:" + crtItem.Data.SkillName;
            skillLv.text = "当前技能等级:" + crtItem.Data.Level.ToString();
            MpCons.text = "耗蓝量:" + crtItem.Data.MpCons.ToString();
            coolTime.text = "冷却时间:" + crtItem.Data.Coolingtime.ToString();
            damage.text = "伤害值:" + crtItem.Data.Damage.ToString();
            descrip.text = "描述信息:" + crtItem.Data.Description;
        }
        /// <summary>
        /// 升级技能
        /// </summary>
        private void SkillLeveUp()
        {
            string tipMessage = null;
            if (SkillManager.I.SkillLeveUp(crtItem.Data, out tipMessage))
            {
                skillPointTip.text = "当前技能点：" + SkillManager.I.skillpoint;
                crtItem.UpDateUI();
                UpDateDescriptionWindow();
                Alert.Show("技能升级成功", tipMessage);
            }
            else
            {
                Alert.Show("技能升级失败", tipMessage);
            }
        }
        #endregion

        #region 技能配置窗口相关
        /// <summary>
        /// 打开技能配置窗口
        /// </summary>
        private void OpenSkillConfigWindow()
        {
            if (crtItem.Data.Level <= 0) { Alert.Show("无法配置", "当前技能还没有到达1级"); return; }
            skillConfigWindow.SetActive(true);
            // 读取技能配置
            skill1 = SkillManager.I.GetSkill(PlayerPrefs.GetString("skill1"));
            skill2 = SkillManager.I.GetSkill(PlayerPrefs.GetString("skill2"));
            skill3 = SkillManager.I.GetSkill(PlayerPrefs.GetString("skill3"));
            // 找到技能面板下的PlayerInputButton
            btns = transform.GetComponentsInChildren<PlayerInputButton>();
            foreach (PlayerInputButton btn in btns)
            {
                //设置默认图标
                btn.cachedImage.overrideSprite= ResourceManager.Load<Sprite>("None");
                btn.cachedImage.rectTransform.sizeDelta = SkillManager.I.iconSize;
                btn.downButton.onClick.AddListener(delegate { SetSkill(btn); });
                // 更新技能图标
                if (skill1 != null && btn.buttonName == "skill1")
                    SkillManager.I.SetButtonIcon(btn, skill1);
                if (skill2 != null && btn.buttonName == "skill2")
                    SkillManager.I.SetButtonIcon(btn, skill2);
                if (skill3 != null && btn.buttonName == "skill3")
                    SkillManager.I.SetButtonIcon(btn, skill3);
            }
            //更新技能配置信息
            currentskillName.text = crtItem.Data.SkillName;
            //相关事件绑定
            checkButton.onClick.AddListener(SaveConfigInfo);
            configCloseButton.onClick.AddListener(CloseSkillConfigWindow);
        }
        /// <summary>
        ///  设置技能信息
        /// </summary>
        /// <param name="btn"></param>
        private void SetSkill(PlayerInputButton btn)
        {
            // 设置技能
            Skill skill = crtItem.Data;
            //如果该键已经有技能存储
            if (PlayerPrefs.HasKey(btn.buttonName))
            {
                //删除该键中所存储的技能名称在其他的键中也有的键
                foreach (var eveBtn in btns)
                {
                    if (eveBtn == btn) continue;
                    if (PlayerPrefs.HasKey(eveBtn.buttonName)
                        && PlayerPrefs.GetString(eveBtn.buttonName) == PlayerPrefs.GetString(btn.buttonName))
                    {
                        eveBtn.GetComponent<Image>().overrideSprite = ResourceManager.Load<Sprite>("None");
                        eveBtn.cachedImage.rectTransform.sizeDelta = SkillManager.I.iconSize;
                        PlayerPrefs.DeleteKey(eveBtn.buttonName);
                    }
                }
            }
            PlayerPrefs.SetString(btn.buttonName, skill.SkillID);
            btn.cachedImage.overrideSprite = skill.SkillIcon;
            btn.cachedImage.rectTransform.sizeDelta = SkillManager.I.iconSize;

        }
        private void CloseSkillConfigWindow()
        {
            skillConfigWindow.SetActive(false);
            //非保存模式
            if (!isSave)
            {
                foreach (var btn in btns)
                {
                    if (PlayerPrefs.HasKey(btn.buttonName))
                    {
                        PlayerPrefs.DeleteKey(btn.buttonName);
                    }
                }
            }
            checkButton.onClick.RemoveListener(SaveConfigInfo);
            configCloseButton.onClick.RemoveListener(CloseSkillConfigWindow);
            //更新玩家配置
            PlayerManager.I.playerTrans.GetComponent<PlayerAttack>().SetSkillConfig();
        }

        /// <summary>
        /// 保存配置信息设置
        /// </summary>
        private void SaveConfigInfo()
        {
            isSave = true;
            CloseSkillConfigWindow();
        }
        #endregion

    }
}
