using UnityEngine;

namespace DialogueSystem
{
    [CreateAssetMenu(fileName = "character info", menuName = "RPG GAME/Character/new character info")]
    public class CharacterInformation : ScriptableObject
    {
        [SerializeField, DisplayName("角色ID")]
        protected string _ID;
        public string ID => _ID;

        [SerializeField, DisplayName("角色姓名")]
        protected string _Name;
        public string Name { get { return _Name; } set { _Name = value; } }

        [SerializeField]
#if UNITY_EDITOR
        [DisplayName("角色类别", false, true, "无", "人族", "精灵", "兽人", "魔族")]
#endif
        protected CharacterType chType;
        public CharacterType ChType => chType;
        /// <summary>
        /// 获取角色性别
        /// </summary>
        /// <param name="sex"></param>
        /// <returns></returns>
        public static string GetSexString(CharacterType sex)
        {
            switch (sex)
            {
                case CharacterType.None:
                    return "无";
                case CharacterType.Elves:
                    return "精灵";
                case CharacterType.Devil:
                    return "魔族";
                case CharacterType.Orcs:
                    return "兽人";
                case CharacterType.People:
                default:
                    return "人族";
            }
        }
    }
    public enum CharacterType
    {
        [InspectorName("无")]
        None,
        [InspectorName("人族")]
        People,
        [InspectorName("精灵")]
        Elves,
        [InspectorName("兽人")]
        Orcs,
        [InspectorName("魔族")]
        Devil
    }
}