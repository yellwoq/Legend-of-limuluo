using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    [RequireComponent(typeof(Image), typeof(Button))]
    public class PlayerInputButton : MonoBehaviour
    {
        #region 属性
        [DisplayName("按钮名字")]
        public string buttonName;
        [DisplayName("键盘输入")]
        public KeyCode keycode;
        [SerializeField, DisplayName("是否自动绑定")]
        private bool isAutoLink;
        [SerializeField, ConditionalHide("操作层级", "isAutoLink", true, false)]
        public LayerMask playerlayer;
        [SerializeField, ConditionalHide("玩家", "isAutoLink", true, true)]
        public Transform playerTrans;
        [HideInInspector]
        public Button downButton;
        [HideInInspector]
        public Image cachedImage;
        #endregion
        #region Unity事件
        private void Awake()
        {
            cachedImage = transform.GetComponent<Image>();
            downButton = transform.GetComponent<Button>();
        }
        private void Update()
        {
            if (isAutoLink)
            {
                playerlayer = LayerMask.GetMask("Player");
            }
            else
            {
                playerTrans = PlayerManager.I.playerTrans;
            }
        }
        #endregion

    }
}

