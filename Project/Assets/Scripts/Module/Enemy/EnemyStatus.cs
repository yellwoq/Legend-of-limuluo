using AI.FSM;
using Bag;
using Common;
using MVC;
using QuestSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public delegate void EnermyDeathListener();
    /// <summary>
    /// 敌人状态信息
    /// </summary>
    [System.Serializable]
    public class EnemyStatus : CharacterStatus, IResetable
    {
        [SerializeField]
        private EnemyInformation enemyInfo;
        public EnemyInformation EenemyInfo => enemyInfo;
        [Header("敌人属性")]
        public int enemyID;
        [SerializeField]
        public string enemyName;
        [SerializeField]
        public string description;
        [SerializeField, Header("其他属性")]
        public float horizontalValue;
        [SerializeField]
        public float vertcalValue;
        [SerializeField]
        public float sightDistance = 3;
        /// <summary>
        /// 敌人死亡监听事件
        /// </summary>
        public event EnermyDeathListener OnDeathEvent;

        public override void Death()
        {
            GetEnemyDeathReward();

            OnDeathEvent?.Invoke();
            QuestManager.Instance.UpdateObjectivesText();
        }
        /// <summary>
        /// 获得奖励
        /// </summary>
        private void GetEnemyDeathReward()
        {
            GameObjectPool.I.CollectObject(transform.parent.gameObject);
            if (enemyInfo.CanDropQuestItem)
            {
                List<int> canCreateItemID = new List<int>();
                bool canDropQuestItem = QuestManager.I.QuestsOngoing.FindAll(
                    e =>
                    {
                        if (e.CollectObjectives == null) return false;
                        bool flag = false;
                        foreach (var itemId in enemyInfo.QuestItems)
                        {
                            if (e.CollectObjectives.Find(co => co.ItemID == itemId.ToString()) != null)
                            {
                                canCreateItemID.Add(itemId);
                                flag = true;
                                continue;
                            }
                        }
                        return flag;
                    }).Count > 0;
                if (canDropQuestItem)
                {
                    int itemCanCreateID = Random.Range(0, 3);
                    if (itemCanCreateID >= 1)
                    {
                        int itemIndex = canCreateItemID[Random.Range(0, canCreateItemID.Count)];
                        if (ItemInfoManager.I.objectInfoDict.ContainsKey(itemIndex))
                        {
                            GameObject go = Instantiate(BagPanel.I.gameItemPrefab, BagPanel.I.gameItemList.transform);
                            go.transform.position = transform.position;
                            go.GetComponent<GameItem>().SetGameItem(itemIndex.ToString());
                        }
                    }
                }
            }
            int nID = Random.Range(0, 3);
            if (nID >= 2)
            {
                int normalItemIndex = Random.Range(0, enemyInfo.KillyRewards.ItemRewards.Count);
                if (ItemInfoManager.I.objectInfoDict.ContainsKey(normalItemIndex))
                {
                    GameObject go = Instantiate(BagPanel.I.gameItemPrefab, BagPanel.I.gameItemList.transform);
                    go.transform.position = transform.position;
                    go.GetComponent<GameItem>().SetGameItem(normalItemIndex.ToString());
                }
            }
            //TODO 经验和金钱的处理
            UserHeroVO newUserHeroVO = GameController.I.crtHero;
            newUserHeroVO.money +=Random.Range(0,enemyInfo.KillyRewards.Money);
            newUserHeroVO.currentExp += enemyInfo.KillyRewards.EXP;
            if (newUserHeroVO.currentExp >= newUserHeroVO.nextLvNeedExp)
            {
                newUserHeroVO.lv++;
                newUserHeroVO.currentExp = newUserHeroVO.currentExp - newUserHeroVO.nextLvNeedExp;
                SkillSystem.SkillManager.I.skillpoint++;
            }
            GameController.I.crtHero = newUserHeroVO;
        }

        public void OnReset()
        {
            currentHP = enemyInfo.MaxHP;
        }
    }
}
