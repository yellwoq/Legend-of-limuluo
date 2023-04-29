using UnityEngine;

public class UIRootManager : MonoBehaviour
{
    /// <summary>
    /// 在加载时不要销毁
    /// </summary>
    public static bool DontDestroyOnLoadOnce;
    void Start()
    {
        if (!DontDestroyOnLoadOnce)
        {
            DontDestroyOnLoad(this);
            DontDestroyOnLoadOnce = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
