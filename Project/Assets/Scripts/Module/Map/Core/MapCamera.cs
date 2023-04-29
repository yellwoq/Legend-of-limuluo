using UnityEngine;

namespace MapSystem
{
    /// <summary>
    /// 地图相机
    /// </summary>
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent]
    public class MapCamera : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Camera myCamera;

        public Camera Camera => myCamera;

        /// <summary>
        /// 启动的时候执行两次，Stop的时候执行一次，组件数值改变的时候执行一次
        /// </summary>
        private void OnValidate()
        {
            myCamera = GetComponent<Camera>();
            myCamera.hideFlags = HideFlags.NotEditable;
            myCamera.orthographic = true;
        }

        private void Awake()
        {
            if (!myCamera) myCamera = GetComponent<Camera>();
            myCamera.hideFlags = HideFlags.NotEditable;
            myCamera.orthographic = true;
        }
    }
}
