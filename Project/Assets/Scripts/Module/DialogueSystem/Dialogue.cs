using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS0649

namespace DialogueSystem
{
    /// <summary>
    /// 单段对话类
    /// </summary>
    [CreateAssetMenu(fileName = "dialogue", menuName = "RPG GAME/Plot/new Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        private Sprite defaultSprite;
        public Sprite DefaultSprite=> defaultSprite;
        [SerializeField]
        private string _ID;
        public string ID => _ID;
        [SerializeField,NonReorderable]
        private List<TalkerInformation> talkers;
        public List<TalkerInformation> Talkers=> talkers;
        [SerializeField,NonReorderable]
        private List<DialogueWords> words;
        public List<DialogueWords> Words => words;
    }
    /// <summary>
    /// 单条对话
    /// </summary>
    [System.Serializable]
    public class DialogueWords
    {
        [SerializeField]
        private TalkerInformation talkerInfo;
        public TalkerInformation TalkerInfo => talkerInfo;
        public Sprite TalkerIcon=> talkerInfo.HeadIcon;
        public string TalkerName => talkerInfo.Name;

        [SerializeField, TextArea]
        private string words;
        public string Words => words;
    }
}