using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common;
using MVC;
using System;
using Components;

namespace UI
{
    /// <summary>
    ///  UI管理器:获取面板,切换面板显示或隐藏,切换多个对象的显示或隐藏
    /// </summary>
    public class UIManager : MonoSingleton<UIManager>
    {
        /// <summary>
        ///  初始化
        /// </summary>
        protected override void Initialize()
        {
            mainCanvas = GameObject.FindGameObjectWithTag( "MainCanvas" ).transform;
            panels = new Dictionary<string , GameObject>();
            objects = new Dictionary<string , GameObject[]>();
            // 遍历枚举类Tags
            Array tagArr = Enum.GetValues( typeof( Common.Tags ) );
            foreach( var t in tagArr )
            {
                string tag = t.ToString();
                objects[ tag ] = GameObject.FindGameObjectsWithTag( tag ); // 自动添加对象
            }
        }
        /// <summary>
        /// 主相机
        /// </summary>
        private Transform mainCanvas;
        /// <summary>
        ///  存储所有面板
        /// </summary>
        private Dictionary<string , GameObject> panels;

        /// <summary>
        ///  获取面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="name">面板名</param>
        /// <param name="canvas">画布</param>
        /// <returns></returns>
        public T GetPanel<T>( string name = null, Transform canvas=null)
        {
            string key = name == null ? typeof( T ).Name : name;
            Transform canvasTrans = canvas == null ? mainCanvas : canvas;
            // 先判断缓存里面有没有
            if ( !panels.ContainsKey( key ) ) // 如果没有
                panels[ key ] = canvasTrans.FindChildByName( key ).gameObject;  // 找出对象,缓存一下
            return panels[ key ].GetComponent<T>();
        }

        /// <summary>
        /// 切换面板隐藏或隐藏
        /// </summary>
        /// <param name="name">面板名称(继承BasePanel)</param>
        /// <param name="active">显示或隐藏</param>
        public void TogglePanel(string name, bool active, Transform canvas = null)
        {
            BasePanel p =  GetPanel<BasePanel>( name,canvas );
            if ( active ) p.Show();
            else p.Hide();
        }
        /// <summary>
        /// 切换指定类型面板显示或隐藏
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="name">面板名</param>
        /// <param name="active">状态</param>
        public void TogglePanel<T>(bool active, string name = null, Transform canvas = null) where T:IView
        {
            T p = GetPanel<T>(name,canvas);
            if (active) p.Show();
            else p.Hide();
        }
        /// <summary>
        /// 切换指定的面板群开关
        /// </summary>
        /// <param name="panels">面板群</param>
        /// <param name="active">状态</param>
        /// <param name="canvas">画布</param>
        public void TogglePanels(Panels[] panels,bool active,Transform canvas = null)
        {
            foreach (var panel in panels)
            {
                TogglePanel(panel, active, canvas);
            }
        }
        /// <summary>
        /// 隐藏指定的子面板的显示或隐藏
        /// </summary>
        /// <param name="parentName">父面板名</param>
        /// <param name="childName">子面板名</param>
        /// <param name="active">状态</param>
        public void TogglePanelInChild(string parentName, string childName,bool active)
        {
            BasePanel p = GetPanel<BasePanel>(parentName);
            BasePanel target=TransformHelper.FindChildByName(p.transform, childName).GetComponent<BasePanel>();
            if (active) target.Show();
            else target.Hide();
        }
        /// <summary>
        /// 从指定的父面板中获取子面板
        /// </summary>
        /// <typeparam name="T">子面板</typeparam>
        /// <param name="parentName">父面板名</param>
        /// <param name="childName">子面板名</param>
        /// <returns>子面板</returns>
        public T GetPanelInChild<T>(string parentName, string childName=null)
        {
            string key = name == null ? typeof(T).Name : name;
            BasePanel p = GetPanel<BasePanel>(parentName);
            T target = p.transform.FindChildByName(childName).GetComponent<T>();
            return target;
        }
        /// <summary>
        /// 切换面板显示或隐藏
        /// </summary>
        /// <param name="p"></param>
        /// <param name="active"></param>
        public void TogglePanel( Panels p, bool active, Transform canvas = null)
        {
            TogglePanel( p.ToString() , active,canvas );
        }
        /// <summary>
        ///  存储指定tag对象
        /// </summary>
        private Dictionary<string , GameObject[]> objects;
        /// <summary>
        ///  切换指定tag对象的显示和隐藏
        /// </summary>
        /// <param name="tag">标签名</param>
        /// <param name="active">显示或隐藏</param>
        public void ToggleObjects( string tag, bool active )
        {
            foreach ( GameObject go in objects[tag] )
                go.SetActive( active );
        }
        /// <summary>
        ///  切换指定tag对象的显示和隐藏
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="active">显示或隐藏</param>
        public void ToggleObjects( Common.Tags tag, bool active )
        {
            ToggleObjects( tag.ToString() , active );
        }
        
    }
}
