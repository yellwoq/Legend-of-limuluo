using Common;
using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    /// 范围技能
    /// </summary>
    public class RangeSkillController : MonoBehaviour
    {
        [HideInInspector]
        public Direction direction;
        [DisplayName("目标标签")]
        public string[] tags = { "Enemy" };
        /// <summary>
        /// 圆形检测单个敌人
        /// </summary>
        /// <param name="attacked">被攻击者</param>
        /// <param name="skillPostion">技能的位置</param>
        /// <param name="radius">半径</param>
        /// <returns></returns>
        public bool CircleAttack(Transform attacker, Transform attacked, float radius)
        {
            float distance = Vector3.Distance(attacked.position, attacker.position);
            if (distance <= radius)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// 攻击扇形范围内的所有敌人
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="angle">角度</param>
        /// <param name="radius">半径</param>
        public void UmbrellaAttact(Transform attacker, float angle, float radius,float damageValue, float delayTime)
        {
            List<CharacterStatus> allTarget = GetAllTarget();
            
            foreach (var enemy in allTarget)
            {
                if (UmbrellaAttact(attacker, enemy.transform, angle, radius))
                {
                    StartCoroutine(DoAttack(enemy, damageValue, delayTime));
                }
            }
        }
        /// <summary>
        /// 圆形范围攻击
        /// </summary>
        /// <param name="attackerPos"></param>
        /// <param name="radius"></param>
        public void CircleAttack(Transform attackerPos,float radius,float damageValue,float delayTime)
        {
            List<CharacterStatus> allTarget = GetAllTarget();
            foreach (var enemy in allTarget)
            {
                if (CircleAttack(attackerPos, enemy.transform, radius))
                {
                    StartCoroutine(DoAttack(enemy, damageValue, delayTime));
                }
            }
        }
        IEnumerator DoAttack(CharacterStatus enemy,float damageValue,float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            enemy.TakeDamage(damageValue);
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

        /// <summary>
        /// 扇形攻击范围内的单个敌人
        /// </summary>
        /// <param name="attacker">攻击者</param>
        /// <param name="attacked">被攻击方</param>
        /// <param name="angle">扇形角度</param>
        /// <param name="radius">扇形半径</param>
        /// <returns></returns>
        public bool UmbrellaAttact(Transform attacker, Transform attacked, float angle, float radius)
        {
            //计算两者之间的距离
            Vector3 deltaA = attacked.position - attacker.position;
            //Mathf.Rad2Deg : 弧度值到度转换常度
            //Mathf.Acos(f) : 返回参数f的反余弦值
            float tmpAngle = Mathf.Acos(Vector3.Dot(deltaA.normalized,attacker.position.normalized)) * Mathf.Rad2Deg;
            //如果夹角满足且距离小于半径
            if (tmpAngle <= angle && deltaA.magnitude <= radius)
            {
                return true;
            }
            return false;
        }
    }
}
