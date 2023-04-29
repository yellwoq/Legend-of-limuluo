using UnityEngine;

namespace Common
{
    [System.Serializable]
    public class CharacterStatus : MonoBehaviour
    {
        [DisplayName("动画参数列表"),Space, Header("角色动画参数"),SerializeField]
        public CharacterAnimationParameter chParams;
        [DisplayName("攻击力", true), Space, Header("角色公共重要属性"),SerializeField]
        public float DEM = 5;
        [DisplayName("防御力", true), SerializeField]
        public float DEF = 5;
        [DisplayName("当前生命值", true), SerializeField]
        public float currentHP = 500;
        [DisplayName("最大生命值", true), SerializeField]
        public float maxHP = 500;
        [DisplayName("速度"), Space, Header("角色公共基础属性"), SerializeField]
        public float moveSpeed = 5;
        [DisplayName("等级",true), SerializeField]
        public int Lv = 1;

        /// <summary>
        ///  承受伤害
        /// </summary>
        /// <param name="damage"></param>
        public virtual void TakeDamage(float damage)
        {
            damage -= DEF*0.5f;
            if (damage <= 0) damage = 1;
            currentHP -= damage;
            Debug.Log("TakeDamage: " + currentHP);
            if (currentHP < 0)
            {
                currentHP = 0;
               Death();
            }
        }

        public virtual void Death()
        {
            GameObjectPool.I.CollectObject(gameObject);
        }
    }
}