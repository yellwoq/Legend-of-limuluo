using UnityEngine;

namespace Player
{

    [RequireComponent(typeof(Camera))]
    public class CameraFollowing2D : SingletonMonoBehaviour<CameraFollowing2D>
    {
        [DisplayName("所要使用的相机")]
        public Camera usecamera;

        [HideInInspector]
        public Transform CameraTransform { get; private set; }
        [DisplayName("跟踪目标")]
        public Transform target;
        [DisplayName("偏移量")]
        public Vector2 offset;
        [DisplayName("是否平滑移动")]
        public bool smooth = true;

#if UNITY_EDITOR
        [ConditionalHide("smooth", true)]
#endif
        public float smoothness = 0.25f;
        [DisplayName("更新方式")]
        public UpdateMode updateMode = UpdateMode.LateUpdate;

        private void Awake()
        {
            if (!usecamera) usecamera = GetComponent<Camera>();
            CameraTransform = usecamera.transform;
        }
        private void OnEnable()
        {
            target = PlayerManager.I.playerTrans;
            usecamera.orthographicSize = 2;
        }
        private void Update()
        {
            if (updateMode == UpdateMode.Update) Follow();
        }

        private void FixedUpdate()
        {
            if (updateMode == UpdateMode.FixedUpdate) Follow();
        }

        void LateUpdate()
        {
            if (updateMode == UpdateMode.LateUpdate) Follow();
        }
        /// <summary>
        /// 设置目标
        /// </summary>
        /// <param name="target"></param>
        /// <param name="offset"></param>
        public void SetTarget(Transform target, Vector2 offset)
        {
            this.target = target;
            this.offset = offset;
        }

        void Follow()
        {
            if (target && CameraTransform)
            {
                if (smooth) CameraTransform.position = Vector3.Lerp(CameraTransform.position, (Vector3)offset + new Vector3(target.position.x, target.position.y, CameraTransform.position.z), smoothness);
                else CameraTransform.position = (Vector3)offset + new Vector3(target.position.x, target.position.y, CameraTransform.position.z);
            }
        }
    }
}
