using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
#pragma warning disable CS0618
namespace Common
{

    /// <summary>
    /// 配置文件读取器
    /// </summary>
    public class ConfigurationReader
    {
        /// <summary>
        /// 获取配置文件内容
        /// </summary>
        /// <param name="path">文件在StreamingAssets的相对目录</param>
        /// <returns></returns>
        public static string GetConfigFile(string path)
        {
            //配置文件路径
            string configFile;
#if UNITY_EDITOR || UNITY_STANDALONE
            configFile = "file://" + Application.dataPath + "/StreamingAssets/" + path;
#elif UNITY_IPHONE
                                  configFile = "file://" + Application.dataPath + "/Raw/" + path;
#elif UNITY_ANDROID
                                   configFile = "jar:file://" + Application.dataPath + "!/assets/" + path;
#endif
            WWW www = new WWW(configFile);
            while (true)
            {
                if (www.isDone)
                    return www.text;
            }
        }
        /// <summary>
        /// 以指定的逻辑加载配置文件
        /// </summary>
        /// <typeparam name="T">配置文件的数据结构</typeparam>
        /// <param name="configFileContent">配置文件内容</param>
        /// <param name="data">需要加载到的对象</param>
        /// <param name="lineHandle">处理逻辑</param>
        public static void ReadConfig(string configFileContent, Action<string> lineHandle)
        {
            using (StringReader reader = new StringReader(configFileContent))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    lineHandle(line);
                }
            }
        }
    }
}
