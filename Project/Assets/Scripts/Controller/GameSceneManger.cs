using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏场景管理
/// </summary>
public class GameSceneManger: MonoBehaviour
{

    private static bool dontDestroyOnLoadOnce;
    /// <summary>
    /// 当前实例
    /// </summary>
    public static GameSceneManger instance;

    private void Start()
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
    }
}
