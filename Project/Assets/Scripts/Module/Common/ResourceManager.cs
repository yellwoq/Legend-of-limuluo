using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// Resources的强化类,资源加载器,图集加载
    /// </summary>
    public class ResourceManager : MonoSingleton<ResourceManager>
    {
        /// <summary>
        /// 配置表映射
        /// </summary>
        private static Dictionary<string, string> configMap;
        /// <summary>
        /// 资源缓存映射
        /// </summary>
        private static Dictionary<string, Object> resCache;

        static ResourceManager()
        {
            configMap = new Dictionary<string, string>();
            resCache = new Dictionary<string, Object>();
            LoadConfig();
        }
        /// <summary>
        /// 资源映射文件路径
        /// </summary>
        public const string FILE_NAME = "Config/ResMap.txt";

        /// <summary>
        ///  加载资源映射文件 
        /// </summary>
        private static void LoadConfig()
        {
            string mapText = ConfigurationReader.GetConfigFile(FILE_NAME);

            ConfigurationReader.ReadConfig(mapText, BuildMap);
        }
        /// <summary>
        /// 加载逻辑
        /// </summary>
        /// <param name="line"></param>
        private static void BuildMap(string line)
        {
            var keyValue = line.Split('=');
            configMap.Add(keyValue[0], keyValue[1]);
        }

        /// <summary>
        /// 通过资源名取得资源路径 
        /// </summary>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        private static string GetValue(string resName)
        {
            if (configMap.ContainsKey(resName))
                return configMap[resName];
            return null;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <typeparam name="T">返回的资源类型</typeparam>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        private static T GetResource<T>(string relativePath) where T : Object
        {
            //如果资源缓存对象中不存在
            if (!resCache.ContainsKey(relativePath))
            {
                T obj = Resources.Load<T>(relativePath);
                if (!obj) return null;//如果没有该资源 返回空
                resCache.Add(relativePath, obj);
                return obj;
            }
            return resCache[relativePath] as T;
        }

        /// <summary>
        /// 在Resources文件夹中加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源名称</param>
        /// <returns></returns>
        public static T Load<T>(string resName) where T : Object
        {
            string path = GetValue(resName);
            if (string.IsNullOrEmpty(path)) return null;//如果配置文件没有该记录 返回空
            return GetResource<T>(path);
        }
        /// <summary>
        /// 读取所有相同资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> LoadAll<T>() where T : Object
        {
            List<T> tAllResource = new List<T>();
            foreach (var keymap in configMap)
            {
                Object o = Load<Object>(keymap.Key);
                if (o is T)
                {
                    tAllResource.Add(o as T);
                }
            }
            return tAllResource;
        }
        /// <summary>
        ///  获取装备Sprite
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Sprite GetEquipSprite(string name)
        {
            return GetAtlasSprite("Equip", name);
        }
        // 加载图集
        /// <summary>
        ///  图集的缓存
        /// </summary>
        private Dictionary<string, UnityEngine.Object[]> atlasDic =
            new Dictionary<string, UnityEngine.Object[]>();
        /// <summary>
        ///  从图集获取Sprite
        /// </summary>
        /// <param name="atlasPath">图集路径</param>
        /// <param name="name">Sprite的名称</param>
        /// <returns></returns>
        public Sprite GetAtlasSprite(string atlasPath, string name)
        {
            UnityEngine.Object[] atlas;
            if (atlasDic.ContainsKey(atlasPath)) // 如果缓存里面有
                atlas = atlasDic[atlasPath];
            else // 缓存没有
            {
                atlas = Resources.LoadAll(atlasPath); // 去加载
                atlasDic[atlasPath] = atlas; // 放到缓存中
            }
            return GetSpriteFromAtlas(atlas, name);
        }
        /// <summary>
        ///  Sprite缓存
        /// </summary>
        private Dictionary<string, Sprite> sprites =
            new Dictionary<string, Sprite>();
        /// <summary>
        ///  从图集中找到指定Sprite
        /// </summary>
        /// <param name="atlas">图集对象</param>
        /// <param name="name">Sprite名称</param>
        /// <returns></returns>
        public Sprite GetSpriteFromAtlas(Object[] atlas, string name)
        {
            // 从缓存中查找
            if (sprites.ContainsKey(name))
                return sprites[name];
            // 从atlas查找
            for (int i = 0; i < atlas.Length; i++)
            {
                if (atlas[i].GetType() == typeof(Sprite)
                    && atlas[i].name == name)
                {
                    Sprite sp = (Sprite)atlas[i];
                    sprites[name] = sp;
                    return sp;
                }
            }
            // 没有找到
            Debug.LogError("图片名: " + name + "在图集中找不到!");
            return null;
        }
    }
}
