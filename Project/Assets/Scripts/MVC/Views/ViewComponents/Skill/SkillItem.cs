using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillSystem;

namespace MVC
{
    /// <summary>
    ///  单个技能框
    /// </summary>
    public class SkillItem : BaseMono
    {
        private Image skillIcon;
        [HideInInspector]
        public Image coverImage;
        private Image selectedBorder;
        private Text skillLv;
        private void Awake()
        {
            skillIcon =GetComponent<Image>();
            coverImage = Find<Image>("coverImg");
            skillLv = Find<Text>("skillLv");
            selectedBorder = Find<Image>("selectedBorder");
        }
        // Data
        private Skill data;
        public Skill Data
        {
            set
            {
                data = value;
                UpDateUI();
            }
            get=> data;
        }
        /// <summary>
        /// 设置是否选择
        /// </summary>
        public bool Selected
        {
            set { selectedBorder.gameObject.SetActive( value ); }
        }
        /// <summary>
        /// 更新信息UI
        /// </summary>
        public void UpDateUI()
        {
            skillLv.text = data.Level.ToString();
            GetComponent<Image>().overrideSprite = data.SkillIcon;
            coverImage.overrideSprite = data.SkillIcon;
            skillIcon.sprite = data.SkillIcon;
            if (data.Level != 0)
            {
                coverImage.gameObject.SetActive(false);
            }
            else
            {
                coverImage.gameObject.SetActive(true);
            }
        }
    }
}
