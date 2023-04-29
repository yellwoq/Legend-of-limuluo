using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TarenaMVC;

namespace MVC
{
    /// <summary>
    ///  MVC框架的入口
    /// </summary>
    public class AppFacade : Facade
    {
        /// <summary>
        ///  复写AppFacade Getter
        /// </summary>
        public new static AppFacade I
        {
            get
            {
                if ( instance == null )
                {
                    lock ( sync )
                    {
                        if ( instance == null )
                        {
                            instance = new AppFacade();
                        }
                    }
                }
                return instance as AppFacade;
            }
        }
        /// <summary>
        ///  启动MVC框架
        /// </summary>
        public void Startup()
        {
            Debug.Log( "启动MVC框架" );
            // 注册Proxy
            RegisterProxy( new UserProxy() );
            RegisterProxy( new HeroProxy() );
            RegisterProxy( new BagProxy() );
            // 注册Mediator
            RegisterMediator(new UserMediator());
            RegisterMediator( new HeroMediator());
            RegisterMediator(new BagMediator());
            RegisterMediator( new MainMediator());
            
        }
        /// <summary>
        /// 游戏终止
        /// </summary>
        public void Stop()
        {
            RemoveProxy("HeroProxy");
            RemoveProxy("BagProxy");
            RemoveMediator("UserMediator");
            RemoveMediator("HeroMediator");
            RemoveMediator("MainMediator");
        }
    }
}
