using MVC;
using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    /// 增益性技能
    /// </summary>
    public class UserSkillController : MonoBehaviour
    {
        [HideInInspector]
        public Gaintype mGainType;
        [HideInInspector]
        public float upValue;
        [HideInInspector]
        public float delayTime;
        [HideInInspector]
        public bool isRelased;
        /// <summary>
        /// 技能收益
        /// </summary>
        public void SkillGain()
        {
            switch (mGainType)
            {
                case Gaintype.Damage:
                    StartCoroutine(AttackUP());
                    break;
                case Gaintype.Hp:
                    StartCoroutine(HpUp());
                    break;
            }
        }
        private void Update()
        {
            transform.position = PlayerManager.I.playerTrans.position;
        }
        private IEnumerator HpUp()
        {
            //记录原始生命值
            float originHP = PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().currentHP;
            isRelased = true;
            yield return new WaitForSeconds(delayTime);
            isRelased = false;
            //如果是玩家死亡
            if (FindObjectOfType<PlayerStatus>(true).currentHP<=0)
            {
                PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().currentHP += upValue;
                GameController.I.crtHero.currentHP += upValue;
                PlayerManager.I.playerTrans.gameObject.SetActive(true);
            }
            else
            {
                float currentHP = PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().currentHP;
                float damagedHP = Math.Abs(originHP - currentHP);
                PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().currentHP += damagedHP;
                if(PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().currentHP> PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().maxHP)
                {
                    PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().currentHP = PlayerManager.I.playerTrans.GetComponent<PlayerStatus>().maxHP;
                }
                GameController.I.crtHero.currentHP += damagedHP;
            }
        }
        IEnumerator AttackUP()
        {
            UserHeroVO newUserHeroVO = GameController.I.crtHero;
            FindObjectOfType<PlayerStatus>().DEM += upValue;
            yield return new WaitForSeconds(delayTime);
            FindObjectOfType<PlayerStatus>().DEM -= upValue;
        }
    }
}
