using MVC;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 脚本的单例类
    /// </summary>
    public class MonoSingleton<T> : MonoBehaviour, TarenaMVC.INotifier where T : MonoSingleton<T>
    {
        private static T instance;
        public static T Instance
        {
            get
            {
                if (!instance || !instance.gameObject)
                {
                    //在场景中查找已经存在的对象(说明客户端代码在Awake中调用Instance)
                    instance = FindObjectOfType(typeof(T)) as T;
                    if (instance != null)
                        instance.Initialize();
                    else
                        //创建脚本对象(说明场景中没有存在的对象) 
                        new GameObject("Singleton of " + typeof(T)).AddComponent<T>();//此时立即执行Awake 
                }
                return instance;
            }
        }

        //对象被创建时执行
        protected void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                Initialize();
            }
        }
        public static T I
        {
            get { return Instance; }
        }
        protected virtual void Initialize() { }

        public void SendNotification(string name, object data = null)
        {
            AppFacade.I.SendNotification(name, data);
        }
    }
}