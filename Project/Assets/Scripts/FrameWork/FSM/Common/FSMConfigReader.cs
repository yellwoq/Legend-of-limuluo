using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AI.FSM
{
    /// <summary>
    /// 用于读取配置文件，形成有限状态机相关的数据结构
    /// </summary>
    public class FSMConfigReader
    {
        //外层字典：key 状态名称   value 字典
        //内存字典：key 条件名称   value 状态名称
        public Dictionary<string, Dictionary<string, string>> map;
        private string mainKey;


        public FSMConfigReader(string fileName)
        {
            map = new Dictionary<string, Dictionary<string, string>>();
            string content = ConfigurationReader.GetConfigFile("Config/"+fileName);
            ConfigurationReader.ReadConfig(content, LineHandler);
        }
        /// <summary>
        /// 每行处理逻辑
        /// </summary>
        /// <param name="line"></param>
        private void LineHandler(string line)
        {
            line = line.Trim();
            if (line == "") return;
            if (line.StartsWith("["))
            {
                //如果该行以[开始，表示状态  [Idle]
                mainKey = line.Substring(1, line.Length - 2);
                map.Add(mainKey, new Dictionary<string, string>());
            }
            else
            {
                //表示条件  NoHealth>Dead
                string[] keyValue = line.Split('>');
                map[mainKey].Add(keyValue[0], keyValue[1]);
            } 
        }
    }
}