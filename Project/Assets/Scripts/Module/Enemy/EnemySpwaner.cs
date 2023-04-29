using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    /// <summary>
    /// 敌人生成器
    /// </summary>
    public class EnemySpwaner : MonoBehaviour
    {
        [DisplayName("敌人")]
        public List<GameObject> enemys;
        [DisplayName("产生位置")]
        public List<Transform> spwanPointList;
        
        void Start()
        {
            StartCoroutine(SpwanEnemy());
        }

        IEnumerator SpwanEnemy()
        {
            yield return new WaitForSeconds(1);
            int enemyIndex = Random.Range(0, enemys.Count);
            for (int i = 0; i < spwanPointList.Count; i++)
            {
                if (spwanPointList[i].childCount<=0)
                {
                    GameObjectPool.I.CreateObject("Enemy", enemys[enemyIndex], spwanPointList[i]);
                }
                for (int j = 0; j < spwanPointList[i].childCount; j++)
                {
                   
                    if (!spwanPointList[i].GetChild(j).gameObject.activeSelf)
                    {
                        yield return new WaitForSeconds(10);
                        GameObjectPool.I.CreateObject("Enemy", enemys[enemyIndex], spwanPointList[i]);
                        break;
                    }
                }
                
            }
            StartCoroutine(SpwanEnemy());
        }
    }
}
