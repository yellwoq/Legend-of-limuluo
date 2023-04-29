using Common;
using System.Collections;
using System.IO;
using UnityEngine;

namespace Sound
{
    /// <summary>
    ///  声音管理器
    /// </summary>
    public class SoundManager : MonoSingleton<SoundManager>
    {
        protected override void Initialize()
        {
            // 读取声音设置
            LoadSetting();
        }
        // 单例
        /// <summary>
        ///  文件名称
        /// </summary>
        private string fileName = "setting.json";
        /// <summary>
        ///  文件路径
        /// </summary>
        private string FilePath
        {
            get { return Application.persistentDataPath + "/" + fileName; }
        }
        /// <summary>
        /// 当前设置
        /// </summary>
        public SettingVO setting;
        /// <summary>
        ///  读取声音设置
        /// </summary>
        private void LoadSetting()
        {
            setting = new SettingVO(); // 默认设置
            if (File.Exists(FilePath))
            {
                string s = File.ReadAllText(FilePath);
                Debug.Log(s);
                setting = JsonUtility.FromJson<SettingVO>(s);
            }
            Debug.Log(FilePath);
            // 应用设置
            ChangeBgmVolume(setting.bgmVolume);
            ToggleBgm(setting.bgmEnabled);
        }
        /// <summary>
        ///  保存设置
        /// </summary>
        private void SaveSetting()
        {
            // 生成json字符串
            string s = JsonUtility.ToJson(setting);
            // 写文件
            File.WriteAllText(FilePath, s);
        }
        /// <summary>
        ///  背景音乐
        /// </summary>
        public AudioSource bgm;
        /// <summary>
        ///  背景音乐开关
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleBgm(bool isOn)
        {
            // 保存设置
            setting.bgmEnabled = isOn;
            // 切换背景音乐
            if (isOn && !bgm.isPlaying)
                bgm.Play();
            else if (!isOn && bgm.isPlaying)
                bgm.Stop();
            // 保存文件
            SaveSetting();
        }
        /// <summary>
        ///  调整背景音乐音量
        /// </summary>
        /// <param name="v"></param>
        public void ChangeBgmVolume(float v)
        {
            // 保存设置
            setting.bgmVolume = v;
            // 调整背景音乐音量
            bgm.volume = v;
            // 保存文件
            SaveSetting();
        }
        /// <summary>
        ///  切换音效开关
        /// </summary>
        /// <param name="isOn"></param>
        public void ToggleSfx(bool isOn)
        {
            // 保存设置
            setting.sfxEnabled = isOn;
            // 保存文件
            SaveSetting();
        }
        /// <summary>
        ///  切换音效音量
        /// </summary>
        /// <param name="v"></param>
        public void ChangeSfxVolume(float v)
        {
            // 保存设置
            setting.sfxVolume = v;
            // 保存文件
            SaveSetting();
        }
        // 保存声音设置
        // 播放背景音乐
        // 
        // 播放音效

        /// <summary>
        ///  音效
        /// </summary>
        [System.Serializable]
        public struct SoundTrack
        {
            public string name;
            public AudioClip audio;
        }
        /// <summary>
        ///  所有音效
        /// </summary>
        public SoundTrack[] soundTracks;
        /// <summary>
        ///  根据名称获取音效文件
        /// </summary>
        /// <param name="name">音效名称</param>
        /// <returns></returns>
        private AudioClip GetClip(string audioName, ReadAudioClipType readType = ReadAudioClipType.SoundTrack)
        {
            switch (readType)
            {
                case ReadAudioClipType.SoundTrack:
                    for (int i = 0; i < soundTracks.Length; i++)
                    {
                        if (soundTracks[i].name == audioName)
                            return soundTracks[i].audio;
                    }
                    return null;
                case ReadAudioClipType.ResourcesLoad:
                    return ResourceManager.Load<AudioClip>(audioName);
            }
            return null;
        }
        /// <summary>
        ///  播放菜单等UI音效
        /// </summary>
        public AudioSource sfx;

        /// <summary>
        ///  播放UI音效
        /// </summary>
        /// <param name="name"></param>
        public void PlaySfx(string name, ReadAudioClipType readType = ReadAudioClipType.SoundTrack)
        {
            // 是否启用音效
            if (!setting.sfxEnabled) return;
            // 获取音效
            AudioClip clip = GetClip(name, readType);
            if (clip == null)
            {
                Debug.LogError("音效文件: " + name + " 未找到!");
                return;
            }
            // 设置音效
            sfx.clip = clip;
            // 音效音量
            sfx.volume = setting.sfxVolume;
            // 播放
            sfx.Play();
        }

        /// <summary>
        ///  播放背景音乐
        /// </summary>
        /// <param name="name"></param>
        public void PlayBgm(string name)
        {
            // 是否启用音效
            if (!setting.bgmEnabled) return;
            // 获取音效
            AudioClip clip = ResourceManager.Load<AudioClip>(name);
            if (clip == null)
            {
                Debug.LogError("音效文件: " + name + " 未找到!");
                return;
            }
            // 设置音效
            bgm.clip = clip;
            // 音效音量
            bgm.volume = setting.bgmVolume;
            // 播放
            bgm.Play();
        }
        /// <summary>
        ///  在指定物体上播放音效
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="name">音效名称</param>
        public void PlaySfx(Transform target, string name, ReadAudioClipType readType = ReadAudioClipType.SoundTrack)
        {
            // 是否启用音效
            if (!setting.sfxEnabled) return;
            // 获取音效
            AudioClip clip = GetClip(name, readType);
            if (!clip) // 没有找到音效
            {
                Debug.LogError("音效文件: " + name + " 未找到!");
                return;
            }
            // AudioSource
            AudioSource audioSource = target.GetComponent<AudioSource>();
            // 如果没有
            if (audioSource == null)
            {
                // 添加AudioSource
                audioSource = target.gameObject.AddComponent<AudioSource>();
            }
            // 设置音效
            audioSource.clip = clip;
            // 设置音效音量
            audioSource.volume = setting.sfxVolume;
            // 播放
            audioSource.Play();
        }
        /// <summary>
        ///  延迟播放音效
        /// </summary>
        /// <param name="target">目标</param>
        /// <param name="name">音效名称</param>
        /// <param name="delay">延迟时间</param>
        /// <param name="readType">方式</param>
        /// <returns></returns>
        public IEnumerator PlaySfxDelay(Transform target, string name, float delay, ReadAudioClipType readType = ReadAudioClipType.SoundTrack)
        {
            yield return new WaitForSeconds(delay);
            PlaySfx(target, name, readType);
        }
        public IEnumerator PlaySfxDelay(string name, float delay, ReadAudioClipType readType = ReadAudioClipType.SoundTrack)
        {
            yield return new WaitForSeconds(delay);
            PlaySfx(name, readType);
        }

        public enum ReadAudioClipType
        {
            SoundTrack,
            ResourcesLoad
        }
    }
}