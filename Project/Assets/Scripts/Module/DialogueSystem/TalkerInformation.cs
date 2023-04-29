using QuestSystem;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "npc info", menuName = "RPG GAME/Character/new Npc Info")]
    [System.Serializable]
    public class TalkerInformation : CharacterInformation
    {
        [SerializeField]
        private Sprite headIcon;
        public Sprite HeadIcon => headIcon;
        [SerializeField]
        private Dialogue defaultDialogue;
        public Dialogue DefaultDialogue => defaultDialogue;

        [SerializeField]
        private bool isVendor;
        public bool IsVendor => isVendor;
        [SerializeField]
        private bool isHasQuest;
        [SerializeField,NonReorderable]
        private List<Quest> questsStored = new List<Quest>();
        public List<Quest> QuestsStored => questsStored;
    }
}