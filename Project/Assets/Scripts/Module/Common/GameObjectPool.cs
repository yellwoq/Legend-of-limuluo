using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 可以重置接口
    /// </summary>
    public interface IResetable
    {
        void OnReset();
    }

    /// <summary>
    /// 对象池
    /// </summary>
    public class GameObjectPool : MonoSingleton<GameObjectPool>
    {
        //1.对象池数据结构
        private Dictionary<string, List<GameObject>> cache;

        private new void Awake()
        {
            base.Awake();
            cache = new Dictionary<string, List<GameObject>>();
        }

        //2.创建对象
        /// <summary>
        /// 通过对象池创建对象
        /// </summary>
        /// <param name="key">需要创建的对象种类</param>
        /// <param name="prefab">需要创建的预制件</param>
        /// <param name="pos">创建的位置</param>
        /// <param name="dir">创建的旋转</param>
        /// <returns></returns>
        public GameObject CreateObject(string key, GameObject prefab, Vector3 pos, Quaternion dir)
        {
            //在池中查找 
            GameObject tempGo = FindUsableObject(key);
            //如果没有找到
            if (tempGo == null)
            {
                //创建物体 
                tempGo = Instantiate(prefab);
                //加入池中
                Add(key, tempGo);
                //将通过对象池创建的物体，存入对象池子物体列表中。
                tempGo.transform.SetParent(transform);
            }
            //使用
            UseObject(tempGo, pos, dir);
            return tempGo;
        }
        /// <summary>
        /// 通过对象池创建对象，再指定的父物体下创建
        /// </summary>
        /// <param name="key">需要创建的对象种类</param>
        /// <param name="prefab">需要创建的预制件</param>
        /// <param name="parent">创建的父物体</param>
        /// <returns></returns>
        public GameObject CreateObject(string key, GameObject prefab, Transform parent)
        {
            //在池中查找 
            GameObject tempGo = FindUsableObject(key);
            //如果没有找到
            if (tempGo == null)
            {
                //创建物体 
                tempGo = Instantiate(prefab, parent);
                //加入池中
                Add(key, tempGo);
                //将通过对象池创建的物体，存入对象池子物体列表中。
                tempGo.transform.SetParent(transform);
            }
            //使用
            UseObject(tempGo, parent);
            return tempGo;
        }
        /// <summary>
        /// 对使用对象池中的对象进行初始化
        /// </summary>
        /// <param name="go">所需要使用的游戏对象</param>
        /// <param name="pos">位置</param>
        /// <param name="dir">旋转</param>
        private void UseObject(GameObject go, Vector3 pos, Quaternion dir)
        {
            //先设置位置
            go.transform.position = pos;
            go.transform.rotation = dir;
            //再启用物体
            go.SetActive(true);
            //重置所有需要重置的脚本
            foreach (var item in go.GetComponentsInChildren<IResetable>())
            {
                item.OnReset();
            }
        }
        /// <summary>
        /// 对使用对象池中的对象进行初始化
        /// </summary>
        /// <param name="go"></param>
        /// <param name="parent"></param>
        private void UseObject(GameObject go, Transform parent)
        {
            go.transform.SetParent(parent);
            go.SetActive(true);
            foreach (var item in go.GetComponentsInChildren<IResetable>())
            {
                item.OnReset();
            }
        }
        /// <summary>
        /// 添加没有的游戏对象
        /// </summary>
        /// <param name="key">对象类型</param>
        /// <param name="tempGo">对象实例</param>
        private void Add(string key, GameObject tempGo)
        {
            //如果池中没有键  则 添加键
            if (!cache.ContainsKey(key)) cache.Add(key, new List<GameObject>());
            //将物体加入池中
            cache[key].Add(tempGo);
        }
        /// <summary>
        /// 根据键查找对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private GameObject FindUsableObject(string key)
        {
            if (cache.ContainsKey(key))
            {
                //查找池中禁用的物体
                return cache[key].Find(o => !o.activeSelf);
            }
            return null;
        }

        /// <summary>
        /// 3.即时回收
        /// </summary>
        /// <param name="go">处理的游戏对象</param>
        public void CollectObject(GameObject go)
        {
            go.SetActive(false);
        }

        /// <summary>
        /// 4.延迟回收
        /// </summary>
        /// <param name="go">处理的游戏对象</param>
        /// <param name="delay">延迟时间</param>
        public void CollectObject(GameObject go, float delay)
        {
            StartCoroutine(DelayCollect(go, delay));
        }

        private IEnumerator DelayCollect(GameObject go, float delay)
        {
            yield return new WaitForSeconds(delay);
            CollectObject(go);
        }

        /// <summary>
        /// 5.清空所有的游戏对象
        /// </summary>
        public void ClearAll()
        {
            //将字典中所有键存入集合
            List<string> listKey = new List<string>(cache.Keys);
            foreach (var item in listKey)
            {
                //遍历集合元素 删除字典记录
                Clear(item);
            }
        }
        /// <summary>
        /// 清除指定键的集合
        /// </summary>
        /// <param name="key"></param>
        public void Clear(string key)
        {

            //倒序删除
            for (int i = cache[key].Count - 1; i >= 0; i--)
            {
                Destroy(cache[key][i]);
            }

            //在字典集合中清空当前记录（集合列表）
            cache.Remove(key);
        }
        /// <summary>
        /// 删除指定逻辑的游戏物体
        /// </summary>
        /// <param name="key">游戏物体类型</param>
        /// <param name="handler">处理逻辑</param>
        public void Clear(string key, Func<GameObject, bool> handler)
        {

            //倒序删除
            for (int i = cache[key].Count - 1; i >= 0; i--)
            {
                if (handler(cache[key][i]))
                    Destroy(cache[key][i]);
            }
            if (cache[key] == null)
                //在字典集合中清空当前记录（集合列表）
                cache.Remove(key);
        }
    }
}
