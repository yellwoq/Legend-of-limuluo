using UnityEngine;

namespace SkillSystem
{
    /// <summary>
    /// 直线移动技能控制
    /// </summary>
    public class LineSkillController : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 targetPos;
        [HideInInspector]
        public float damage;
        [DisplayName("目标标签")]
        public string[] tags = { "Enemy" };

        private void FixedUpdate()
        {
            if (targetPos != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.01f);
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (1 << collision.gameObject.layer == LayerMask.GetMask("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy.EnemyStatus>().TakeDamage(damage);
            }
        }
    }
}
