using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI;
using System;
using Localizational;
using Sound;

namespace MVC
{
    /// <summary>
    ///  设置面板
    /// </summary>
    public class SettingPanel : BasePanel
    {
        private Dropdown localeDropdown;
        private Button exitButton;

        protected override void Awake()
        {
            base.Awake();
            localeDropdown = Find<Dropdown>( "localeDropdown" );
            exitButton = Find<Button>("exitButton");
            localeDropdown.onValueChanged.AddListener( ChangeLocale );
            exitButton.onClick.AddListener(ExitGame);
            // 初始化声音组件
            InitSoundControlls();
        }
        /// <summary>
        /// 退出游戏
        /// </summary>
        private void ExitGame()
        {
            AppFacade.I.Stop();
            GameObject[] games=FindObjectsOfType<GameObject>();
            foreach (var item in games)
            {
                Destroy(item);
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            
        }

        /// <summary>
        ///  切换语言
        /// </summary>
        /// <param name="index"></param>
        private void ChangeLocale( int index )
        {
            string localeName = localeDropdown.options[ index ].text;
            LocaleManager.I.ChangeLocale( localeName );
        }
        // 声音组件
        private Toggle bgmToggle;
        private Toggle sfxToggle;
        private Slider bgmSlider;
        private Slider sfxSlider;

        /// <summary>
        /// 初始化声音组件
        /// </summary>
        private void InitSoundControlls()
        {
            bgmToggle = Find<Toggle>( "bgmToggle" );
            sfxToggle = Find<Toggle>( "sfxToggle" );
            bgmSlider = Find<Slider>( "bgmSlider" );
            sfxSlider = Find<Slider>( "sfxSlider" );
        }
        /// <summary>
        /// 当前设置
        /// </summary>
        private void OnEnable()
        {
            // 读取设置
            SettingVO setting = SoundManager.I.setting;
            // 应用设置
            bgmToggle.isOn = setting.bgmEnabled;
            sfxToggle.isOn = setting.sfxEnabled;
            bgmSlider.value = setting.bgmVolume;
            sfxSlider.value = setting.sfxVolume;
            // 监听组件事件
            bgmToggle.onValueChanged.AddListener(SoundManager.I.ToggleBgm);
            bgmSlider.onValueChanged.AddListener(SoundManager.I.ChangeBgmVolume);
            sfxToggle.onValueChanged.AddListener(SoundManager.I.ToggleSfx);
            sfxSlider.onValueChanged.AddListener(SoundManager.I.ChangeSfxVolume);
        }


        private void OnDisable()
        {
            // 移除组件事件监听
            bgmToggle.onValueChanged.RemoveListener(SoundManager.I.ToggleBgm);
            bgmSlider.onValueChanged.RemoveListener(SoundManager.I.ChangeBgmVolume);
            sfxToggle.onValueChanged.RemoveListener(SoundManager.I.ToggleSfx);
            sfxSlider.onValueChanged.RemoveListener(SoundManager.I.ChangeSfxVolume);
        }

    }
}

