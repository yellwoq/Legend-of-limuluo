using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
/*
Unity 编辑器扩展
只在Unity 编辑器中执行，为了增加新功能（扩展菜单，美化Inspector面板，增加窗口）的程序。
注意：
1. 代码只能放在Editor目录中。
2. 代码不会发布到平台中。
*/
/// <summary>
/// 生成资源配置文件
/// </summary>
public class GenerateResConfig
{
    [MenuItem("Tools/Resources/Genrate Resourece Config File")]
    public static void Generate()
    {
        string findStr = "t:Prefab t:Texture2D t:Sprite t:AudioClip t:TalkerInformation t:quest t:SystemHero t:skill t:MapRange t:Dialogue t:EnemyInformation";
        //1. 获取Resources目录中指定类型的资源路径
        string[] resFilePaths = AssetDatabase.FindAssets(findStr, new string[] { "Assets/Resources" });
        if (resFilePaths == null) return;
        for (int i = 0; i < resFilePaths.Length; i++)
        {
            //Assets/Resources/Prefabs/CollisionEffects/Effect6_Collision.prefab
            string assetPath = AssetDatabase.GUIDToAssetPath(resFilePaths[i]);
            //2. 生成对应关系
            //资源名=路径
            string fileName = Path.GetFileNameWithoutExtension(assetPath);
            //Prefabs/CollisionEffects/Effect6_Collision.prefab
            string path = assetPath.Replace("Assets/Resources/", "");
            path = path.Substring(0, path.IndexOf('.'));
            resFilePaths[i] = fileName + "=" + path;
        }
        //3. 写入配置文件
        if (!File.Exists("Assets/StreamingAssets/Config/ResMap.txt"))
        { File.Create("Assets/StreamingAssets/Config/ResMap.txt"); }
        File.WriteAllLines("Assets/StreamingAssets/Config/ResMap.txt", resFilePaths);
        AssetDatabase.Refresh();//刷新
    }
}
