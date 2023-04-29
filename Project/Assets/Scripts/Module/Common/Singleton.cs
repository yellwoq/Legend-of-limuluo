/// <summary>
/// 单例模式
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : new()
{
    protected static T instance;
    protected static readonly object sync = new object();
    protected Singleton()
    {
        Initialize(); // 自动执行初始化
    }

    /// <summary>
    /// 用法：Singleton<myclass>.Instance 获取该类的单例 
    /// </summary>
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (sync)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                }
            }
            return instance;
        }
    }
    /// <summary>
    /// 获取单例
    /// </summary>
    /// <returns></returns>
    public static T I
    {
        get { return Instance; }
    }
    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Initialize()
    {

    }
}
