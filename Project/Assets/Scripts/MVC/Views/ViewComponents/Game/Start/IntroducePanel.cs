using System.Collections.Generic;
using System.IO;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Common;
using StorySystem;

namespace MVC
{
    /// <summary>
    /// 介绍面板
    /// </summary>
    public class IntroducePanel : BasePanel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        private string fileName;
        public string FileSourcePath
        {
            get { return Application.streamingAssetsPath + "/Introduce/" + fileName; }
        }
        public string FileDesPath
        {
            get { return Application.persistentDataPath + "/Introduce/" + fileName; }
        }
        private Text content;
        string[] contentStr;

        StoryAgent startStory;
        protected override void Awake()
        {
            base.Awake();
            content = transform.FindChildComponentByName<Text>("Content");
            //读取文档
            ReadFile("Start.txt");
            Sound.SoundManager.I.PlayBgm("bgmMusic");
        }
        private void OnEnable()
        {
            if (startStory.CurrentFlowchart.GetBooleanVariable("isHasRead"))
            {
                Hide();
            }

        }
        public override void Hide()
        {
            base.Hide();
            Sound.SoundManager.I.PlayBgm("BGM1");
            StoryManager.I.InitStory();
        }
        public void ReadFile(string fileName)
        {
            this.fileName = fileName;
            if (!File.Exists(FileDesPath)) {
                if(!Directory.Exists(FileDesPath))
                Directory.CreateDirectory(Path.GetDirectoryName(FileDesPath));
                File.Copy(FileSourcePath, FileDesPath); }
            contentStr = File.ReadAllLines(FileDesPath);
        }

        public void ChangeText(int textIndex)
        {
            content.text = contentStr[textIndex];
        }
    }
}
