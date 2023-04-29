using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System;

namespace SkillSystem
{
    /// <summary>
    ///  生成技能信息文件
    /// </summary>
    public class SkillUtility
    {
        [MenuItem("Tools/Skill/Create Skill")]
        static void Create()
        {
            //选择的所有文件夹
            string[] selectedGUIDs = Selection.assetGUIDs;
            //存储将转换的路径
            List<string> directoryPaths = new List<string>();
            for (int i = 0; i < selectedGUIDs.Length; i++)
            {
                //转换为路径
                string path = AssetDatabase.GUIDToAssetPath(selectedGUIDs[i]);
                //如果是文件
                if (File.Exists(path))
                    directoryPaths.Add(Path.GetDirectoryName(path));
                else
                    //获得文件夹,加入
                    directoryPaths.Add(path);
            }
            for (int i = 0; i < directoryPaths.Count; i++)
            {
                // 生成SystemHero实例
                Skill heroInfo = ScriptableObject.CreateInstance<Skill>();
                string path = string.Format(directoryPaths[i] + "/skill{0}.asset", i);
                //如果文件已存在
                if (File.Exists(path))
                    path = string.Format(directoryPaths[i] + "/skill{0}.asset", Guid.NewGuid().ToString("N"));
                // 生成文件
                AssetDatabase.CreateAsset(heroInfo, path);
                if (i == 0)
                    // 自动选中第一个
                    Selection.activeObject = heroInfo;
            }
        }
    }
}
