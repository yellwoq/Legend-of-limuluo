using Common;
using MVC;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
/// <summary>
///  整个程序的入口
/// </summary>
public class GameController : MonoSingleton<GameController>
{
    /// <summary>
    /// 当前用户
    /// </summary>
    [HideInInspector]
    public UserVO crtUser;
    /// <summary>
    ///  当前用户英雄
    /// </summary>
    [HideInInspector]
    public UserHeroVO crtHero;
    /// <summary>
    /// 当前实例
    /// </summary>
    public static GameController instance;

    public float calculaterTime = 5;

    public float useItemCDTime = 5;//物品冷却时间

    private static bool dontDestroyOnLoadOnce;
    void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        // 检查数据库
        CheckDB();
    }
    /// <summary>
    ///  数据库名称
    /// </summary>
    private string dbName = "mydata.db";
    /// <summary>
    ///  数据库路径
    /// </summary>
    private string dbPath;
    /// <summary>
    ///  检查数据库
    /// </summary>
    private void CheckDB()
    {
        dbPath = Application.persistentDataPath + "/" + dbName;
        Debug.Log("CheckDB: " + dbPath);
        // 是否存在数据库
        if (!File.Exists(dbPath)) // 如果没有,
            StartCoroutine(CopyDB());// 复制数据
        else // 启动程序
            Startup();
    }
    /// <summary>
    ///  复制数据库
    /// </summary>
    /// <returns></returns>
    private IEnumerator CopyDB()
    {
        // www从StreamingAssets目录下载数据库
        //WWW www = new WWW( Application.streamingAssetsPath + "/" + dbName );
        using (UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + dbName))
        {
            request.SendWebRequest();
            //如果网络错误
            if (request.result == UnityWebRequest.Result.ConnectionError) yield return null;
            yield return request.downloadHandler;// 等待数据库下载完毕
            // 用File.WriteAllBytes将www.bytes写入dbPath
            File.WriteAllBytes(dbPath, request.downloadHandler.data);
            // 启动程序
            Startup();
        }
    }

    private void FixedUpdate()
    {
        calculaterTime += Time.fixedDeltaTime;
    }
    /// <summary>
    ///  正式启动程序
    /// </summary>
    private void Startup()
    {
        Debug.Log("正式启动程序");
        // 限制帧频
        Application.targetFrameRate = 30;
        // 启动MVC框架
        AppFacade.I.Startup();
    }

}
