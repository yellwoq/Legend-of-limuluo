using QuestSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敌人信息
    /// </summary>
    [CreateAssetMenu(fileName ="Enemy",menuName ="RPG GAME/Enemy/new Enemy")]
    public class EnemyInformation : ScriptableObject
    {
        [SerializeField]
        private Sprite enemyIcon;
        public Sprite EnemyIcon => enemyIcon;
        [SerializeField,DisplayName("敌人ID")]
        private int enemyID;
        public int EnemyID=>enemyID;
        [SerializeField, DisplayName("敌人等级")]
        private int enemyLv;
        public int EnemyLv=> enemyLv;
        [SerializeField, DisplayName("敌人姓名")]
        private string enemyName;
        public string EnemyName=>enemyName;
        [SerializeField, DisplayName("敌人最大血量")]
        private float maxHP;
        public float MaxHP => maxHP;
        [SerializeField, DisplayName("攻击力")]
        private float attackPower;
        public float AttackPower => attackPower;
        [SerializeField, DisplayName("防御力")]
        private float defencePower;
        public float DefencePower => defencePower;
        [SerializeField, DisplayName("是否有任务道具需要")]
        private bool canDropQuestItem;
        public bool CanDropQuestItem => canDropQuestItem;
        [SerializeField,DisplayName("任务道具ID")]
        private List<int> questItems;
        public List<int> QuestItems=>questItems;
        [SerializeField, DisplayName("敌人描述")]
        private string enemyDes;
        public string EnemyDes => enemyDes;
        [SerializeField,DisplayName("击杀奖励")]
        private Reward killrewards;

        public Reward KillyRewards => killrewards;
    }
}
