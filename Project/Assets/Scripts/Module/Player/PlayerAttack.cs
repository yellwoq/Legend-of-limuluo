using Common;
using MVC;
using SkillSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    /// <summary>
    ///  玩家技能释放及攻击控制
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        //冷却文本
        private Text skill1cdText;
        private Text skill2cdText;
        private Text skill3cdText;
        //遮罩
        private Image skill1cdImg;
        private Image skill2cdImg;
        private Image skill3cdImg;
        [DisplayName("普攻", true)]
        public Skill normal;
        private Skill skill1;
        private Skill skill2;
        private Skill skill3;

        public PlayerInputButton[] btns;
        private void Awake()
        {
            normal = ResourceManager.Load<Skill>(GameController.I.crtHero.heroType + "normal");
            // 重置技能冷却状态
            if (skill1 != null) skill1.IsCoding = false;
            if (skill2 != null) skill2.IsCoding = false;
            if (skill3 != null) skill3.IsCoding = false;
            SetSkillConfig();
        }
        /// <summary>
        /// 设置技能配置
        /// </summary>
        public void SetSkillConfig()
        {
            // 读取配置
            skill1 = SkillManager.I.GetSkill(PlayerPrefs.GetString(KeyList.SKILL1));
            skill2 = SkillManager.I.GetSkill(PlayerPrefs.GetString(KeyList.SKILL2));
            skill3 = SkillManager.I.GetSkill(PlayerPrefs.GetString(KeyList.SKILL3));
            btns = GameObject.FindGameObjectWithTag("GameCanvas").transform.GetComponentsInChildren<PlayerInputButton>(true);
            foreach (var btn in btns)
            {
                //设置默认图标
                btn.transform.GetComponent<Image>().overrideSprite = ResourceManager.Load<Sprite>("None");
                btn.transform.GetComponent<Image>().rectTransform.sizeDelta = SkillManager.I.iconSize;
                btn.downButton.onClick.RemoveAllListeners();
                //绑定相关事件
                if (1 << gameObject.layer == btn.playerlayer || transform == btn.playerTrans && btn.downButton.onClick == null)
                    btn.downButton.onClick.AddListener(() => OnDown(btn.buttonName));
                if (btn.buttonName == "skill1")
                {
                    // 更新技能图标
                    if (skill1 != null) SkillManager.I.SetButtonIcon(btn, skill1);
                    skill1cdText = btn.transform.FindChildComponentByName<Text>("cdText");
                    skill1cdImg = btn.transform.FindChildComponentByName<Image>("icon");
                }
                if (btn.buttonName == "skill2")
                {
                    if (skill2 != null) SkillManager.I.SetButtonIcon(btn, skill2);
                    skill2cdText = btn.transform.FindChildComponentByName<Text>("cdText");
                    skill2cdImg = btn.transform.FindChildComponentByName<Image>("icon");
                }
                if (btn.buttonName == "skill3")
                {
                    if (skill3 != null) SkillManager.I.SetButtonIcon(btn, skill3);
                    skill3cdText = btn.transform.FindChildComponentByName<Text>("cdText");
                    skill3cdImg = btn.transform.FindChildComponentByName<Image>("icon");
                }
            }
        }
        private void Update()
        {
            PlayerInput();
        }

        private void PlayerInput()
        {
            foreach (var btn in btns)
            {
                if (1 << gameObject.layer == btn.playerlayer || transform == btn.playerTrans)
                    if (Input.GetKeyDown(btn.keycode))
                    {
                        OnDown(btn.buttonName);
                    }
            }

        }
        /// <summary>
        ///  点击按钮释放技能
        /// </summary>
        /// <param name="arg0"></param>
        private void OnDown(string name)
        {
            switch (name)
            {
                case "normal": // 释放普攻技能
                    SkillManager.I.Fire(normal);
                    break;
                case "skill1":// 释放大招1
                    if (skill1 != null && GameController.I.crtHero.currentMP >= skill1.MpCons) SkillManager.I.Fire(skill1, Skill1CD);
                    break;
                case "skill2":// 释放大招2
                    if (skill2 != null && GameController.I.crtHero.currentMP >= skill2.MpCons) SkillManager.I.Fire(skill2, Skill2CD);
                    break;
                case "skill3":// 释放大招3
                    if (skill3 != null && GameController.I.crtHero.currentMP >= skill3.MpCons) SkillManager.I.Fire(skill3, Skill3CD);
                    break;
            }
        }
        /// <summary>
        ///  显示大招冷却倒计时
        /// </summary>
        /// <param name="cd"></param>
        private void Skill1CD(int cd)
        {
            skill1cdText.text = cd > 0 ? cd.ToString() : string.Empty;
            skill1cdImg.gameObject.SetActive(cd > 0);
            skill1cdImg.fillAmount = cd * 1.0f / skill1.Coolingtime;
        }
        /// <summary>
        ///  显示大招2冷却倒计时
        /// </summary>
        /// <param name="cd"></param>
        private void Skill2CD(int cd)
        {
            skill2cdText.text = cd > 0 ? cd.ToString() : string.Empty;
            skill2cdImg.gameObject.SetActive(cd > 0);
            skill2cdImg.fillAmount = cd * 1.0f / skill2.Coolingtime;
        }
        /// <summary>
        ///  显示大招3冷却倒计时
        /// </summary>
        /// <param name="cd"></param>
        private void Skill3CD(int cd)
        {
            skill3cdText.text = cd > 0 ? cd.ToString() : string.Empty;
            skill3cdImg.gameObject.SetActive(cd > 0);
            skill3cdImg.fillAmount = cd * 1.0f / skill3.Coolingtime;
        }

        private void OnDestroy()
        {
            //解除绑定
            foreach (var btn in btns)
            {
                //解绑相关事件
                if (1 << gameObject.layer == btn.playerlayer || transform == btn.playerTrans)
                    btn.downButton.onClick.RemoveListener(() => OnDown(btn.buttonName));
            }
        }
    }
}
