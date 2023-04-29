using System;
using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// 数组处理助手
    /// </summary>
    public static class ArrayHelper
    {
        /// <summary>
        /// 根据对象的属性对对象数组的升序排列
        /// </summary>
        /// <typeparam name="T">对象数组的元素类型 如:Enemy</typeparam>
        /// <typeparam name="TKey">对象的属性 如:HP</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="condition">排序的比较依据</param>
        public static void OrderBy<T, TKey>(this T[] array, Func<T, TKey> condition) where TKey : IComparable<TKey>
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int k = i + 1; k < array.Length; k++)
                {
                    if (condition(array[i]).CompareTo(condition(array[k])) > 0)
                    {
                        T temp = array[i];
                        array[i] = array[k];
                        array[k] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 对象数组的降序序排列
        /// </summary>
        /// <typeparam name="T">对象数组的元素类型 如:Enemy</typeparam>
        /// <typeparam name="TKey">对象的属性 如:HP</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="condition">排序的比较依据</param>
        public static void OrderByDescending<T, TKey>(this T[] array, Func<T, TKey> condition) where TKey : IComparable<TKey>
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int k = i + 1; k < array.Length; k++)
                {
                    if (condition(array[i]).CompareTo(condition(array[k])) < 0)
                    {
                        T temp = array[i];
                        array[i] = array[k];
                        array[k] = temp;
                    }
                }
            }
        }

        public static T GetMax<T, R>(this T[] array, Func<T, R> handler) where R : IComparable
        {
            if (array == null || array.Length == 0) return default(T);

            T max = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                //if (max.HP < array[i].HP)
                //if(XXX(max) < XXX(array[i]))
                //if(handler(max) < handler(array[i]))
                if (handler(max).CompareTo(handler(array[i])) < 0)
                    max = array[i];
            }
            return max;
        }

        public static T GetMin<T, R>(this T[] array, Func<T, R> handler) where R : IComparable
        {
            if (array == null || array.Length == 0) return default(T);

            T min = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (handler(min).CompareTo(handler(array[i])) > 0)
                    min = array[i];
            }
            return min;
        }
        /// <summary>
        /// 获取指定条件的最大元素
        /// </summary>
        /// <typeparam name="T">对象数组的元素类型 如:Enemy</typeparam>
        /// <typeparam name="TKey">对象的属性 如:HP</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="condition">获取条件</param>
        /// <returns></returns>
        public static T Max<T, TKey>(this T[] array, Func<T, TKey> condition) where TKey : IComparable<TKey>
        {
            T max = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (condition(max).CompareTo(condition(array[i])) < 0)
                {
                    max = array[i];
                }
            }
            return max;
        }

        /// <summary>
        /// 获取指定条件的最小元素
        /// </summary>
        /// <typeparam name="T">对象数组的元素类型 如:Enemy</typeparam>
        /// <typeparam name="TKey">对象的属性 如:HP</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="condition">获取条件</param>
        /// <returns></returns>
        public static T Min<T, TKey>(this T[] array, Func<T, TKey> condition) where TKey : IComparable<TKey>
        {
            T t = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (condition(t).CompareTo(condition(array[i])) > 0)
                {
                    t = array[i];
                }
            }
            return t;
        }

        /// <summary>
        /// 查找单个对象
        /// </summary>
        /// <typeparam name="T">对象数组的元素类型 如:Enemy</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="handler">查找条件</param>
        /// <returns></returns>
        public static T Find<T>(this T[] array, Func<T, bool> handler)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (handler(array[i]))
                {
                    return array[i];
                }
            }
            return default(T);
        }

        /// <summary>
        /// 查找满足条件的所有对象
        /// </summary>
        /// <typeparam name="T">对象数组的元素类型 如:Enemy</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="handler">查找条件</param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Func<T, bool> handler)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (handler(array[i]))
                {
                    list.Add(array[i]);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// 筛选对象，将对象转换为对应的类型
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <typeparam name="TKey">筛选后的类型</typeparam>
        /// <param name="array">对象数组</param>
        /// <param name="handler">筛选策略</param>
        /// <returns></returns>
        public static TKey[] Select<T, TKey>(this T[] array, Func<T, TKey> handler)
        {
            TKey[] keys = new TKey[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                keys[i] = handler(array[i]);
            }
            return keys;
        }
    }
}
