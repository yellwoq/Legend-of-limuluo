using Common;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    /// 移动技能
    /// </summary>
    public class MoveSkillController : MonoBehaviour
    {
        [HideInInspector]
        public float damage;
        [HideInInspector]
        public float distance;
        [DisplayName("目标标签")]
        public string[] tags = { "Enemy" };
        [HideInInspector]
        public float delayTime;
        [HideInInspector]
        public Direction dir;
        Enemy.EnemyStatus target;
        [HideInInspector]
        public float damageCountiueTime;
        public IEnumerator MoveAttack(float damage, float distance, Direction dir, float delayTime, float countinueTime)
        {
            yield return new WaitForSeconds(delayTime);
            List<CharacterStatus> allTarget = GetAllTarget();
            Debug.Log(allTarget.Count);
            List<CharacterStatus> newTargets = allTarget.FindAll(e => e.currentHP > 0 && Vector3.Distance(PlayerManager.I.playerTrans.position, e.transform.position) <= distance);
            //条件：找到攻击内的活的最近的
            target = allTarget.ToArray().GetMin(e => Vector3.Distance(PlayerManager.I.playerTrans.position, e.transform.position)) as Enemy.EnemyStatus;
            if (target)
            {
                PlayerManager.I.playerTrans.position = target.transform.position;
                switch (dir)
                {
                    case Direction.Left:
                        PlayerManager.I.playerTrans.position += new Vector3(-1, 0, 0);
                        break;
                    case Direction.Right:
                        PlayerManager.I.playerTrans.position += new Vector3(1, 0, 0);
                        break;
                    case Direction.Up:
                        PlayerManager.I.playerTrans.position += new Vector3(0, 1, 0);
                        break;
                    case Direction.Down:
                        PlayerManager.I.playerTrans.position += new Vector3(0, -1, 0);
                        break;
                }
                StartCoroutine(EnemyTakeDamage(target, damage, distance, delayTime, countinueTime, 0));
            }

           

        }

        IEnumerator EnemyTakeDamage(Enemy.EnemyStatus enemy, float damage, float distance, float delayDamageTime, float countinueTime, float timer)
        {
            yield return new WaitForSeconds(delayDamageTime);
            if (enemy)
            {
                if (timer >= countinueTime || enemy.currentHP <= 0) yield return null;
                else
                {
                    timer += delayDamageTime / 4;
                    float newDistance = Vector3.Distance(
                     PlayerManager.I.playerTrans.position, enemy.transform.position);
                    // 计算伤害值
                    float newdamage = (1 - newDistance / distance) * damage;
                    enemy.TakeDamage(newdamage);
                    StartCoroutine(EnemyTakeDamage(target, damage, distance, delayTime, countinueTime, timer));
                }
            }
        }
        /// <summary>
        /// 获取所有目标
        /// </summary>
        /// <returns></returns>
        private List<CharacterStatus> GetAllTarget()
        {
            List<CharacterStatus> allTarget = new List<CharacterStatus>();
            //根据标签 查找所有目标。
            for (int i = 0; i < tags.Length; i++)
            {
                GameObject[] tempGoArr = GameObject.FindGameObjectsWithTag(tags[i]);
                CharacterStatus[] tempCsArr = tempGoArr.Select(e => e.GetComponentInChildren<CharacterStatus>());
                allTarget.AddRange(tempCsArr);
            }

            return allTarget;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (1 << collision.gameObject.layer == LayerMask.GetMask("Enemy"))
            {
                StartCoroutine(EnemyTakeDamage(collision.GetComponent<Enemy.EnemyStatus>(), damage, distance, delayTime, damageCountiueTime, 0));
            }
        }
    }
}
