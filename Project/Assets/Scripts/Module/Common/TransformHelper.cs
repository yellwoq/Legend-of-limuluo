using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Transform 助手类
    /// </summary> 
    public static class TransformHelper
    {
        /// <summary>
        /// 根据名称查找后代物体
        /// </summary>
        /// <param name="parentTF">父物体Transform</param>
        /// <param name="childName">要查找的名字</param>
        /// <returns></returns>
        public static Transform FindChildByName(this Transform parentTF, string childName)
        {
            Transform child = parentTF.Find(childName);
            if (child != null) return child;

            for (int i = 0; i < parentTF.childCount; i++)
            {
                child = parentTF.GetChild(i);
                var go = FindChildByName(child, childName);
                if (go != null) return go;
            }
            return null;
        }
        /// <summary>
        /// 根据名称查找后代物体中的组件
        /// </summary>
        /// <typeparam name="T">组件类型</typeparam>
        /// <param name="parentTF">父物体Transform</param>
        /// <param name="childName">要查找的名字</param>
        /// <returns></returns>
        public static T FindChildComponentByName<T>(this Transform parentTF, string childName) where T:Component
        {
            Transform child = parentTF.Find(childName);
            if (child != null) return child.GetComponent<T>();

            for (int i = 0; i < parentTF.childCount; i++)
            {
                child = parentTF.GetChild(i);
                var go = FindChildByName(child, childName);
                if (go != null) return go.GetComponent<T>();
            }
            return default(T);
        }
        /// <summary>
        /// 注视目标方向旋转
        /// </summary>
        /// <param name="targetDir">转向的目标</param>
        public static void LookDirection(this Transform tf, Vector3 targetDir,  float rotationSpeed)
        {
            if (targetDir != Vector3.zero)
            {
                Quaternion dir = Quaternion.LookRotation(targetDir);
                tf.rotation = Quaternion.Lerp(tf.rotation, dir, rotationSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// 注视目标点旋转
        /// </summary>
        /// <param name="targetDir">转向的目标</param>
        public static void LookPosition(this Transform tf, Vector3 pos, float rotationSpeed)
        {
            LookDirection(tf, pos - tf.position,rotationSpeed);
        }
    
        /// <summary>
        /// 计算周边物体
        /// </summary>
        /// <param name="currentTF">计算的物体原点</param>
        /// <param name="targetTags">目标标签数组</param>
        /// <param name="distance">距离</param>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static Transform[] CalculateAroundObject(this Transform currentTF, string[] targetTags, float distance, float angle)
        {
            List<Transform> list = new List<Transform>();
            //1根据技能数据定义的攻击目标的tag 选择场景中攻击目标
            for (int i = 0; i < targetTags.Length; i++)
            {
                var allTargets = GameObject.FindGameObjectsWithTag(targetTags[i]);
                if (allTargets != null && allTargets.Length > 0)
                    list.AddRange(ArrayHelper.Select(allTargets, o => o.transform));
            }
            if (list.Count == 0) return null;
            //2从所有攻击目标中找出  在攻击范围内          
            var listNew = list.FindAll(tf =>
                      Vector3.Distance(tf.position, currentTF.position) <= distance &&
                      Vector3.Angle(currentTF.forward, tf.position - currentTF.position) <= angle * 0.5f
            );
            return listNew.ToArray();
        }
        /// <summary>
        /// 删除所有的子节点
        /// </summary>
        /// <param name="parentTF"></param>
        public static void DeleteAllChild(this Transform parentTF)
        {
            if (parentTF.childCount > 0)
            {
                for (int i = 0; i < parentTF.childCount; i++)
                {
                    Object.Destroy(parentTF.GetChild(0).gameObject);
                }
            }
        }
        public static void CollectAllChild(this Transform parentTF)
        {
            if (parentTF.childCount > 0)
            {
                for (int i = 0; i < parentTF.childCount; i++)
                {
                   ObjectPool.Put(parentTF.GetChild(0).gameObject);
                }
            }
        }
    }
}