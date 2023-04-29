using Common;
using DialogueSystem;
using MVC;
using QuestSystem;
using UnityEngine;
namespace Player
{
    /// <summary>
    ///  玩家移动时动画状态切换
    /// </summary>
    public class PlayerMove : MonoBehaviour
    {
        /// <summary>
        /// 动画控制器
        /// </summary>
        private Animator anim;
        private SpriteRenderer srenderer;
        [HideInInspector]
        public ETCJoystick move;
        [DisplayName("最后的移动方向")]
        public Direction lastDir;
        private PlayerStatus status;
        void Awake()
        {
            status = GetComponent<PlayerStatus>();
            anim = GetComponent<Animator>();
            srenderer = GetComponent<SpriteRenderer>();
            anim.SetBool(AnimConst.run, false);
            anim.speed = 0;
            // 获取ETCJoystick
            move = FindObjectOfType<ETCJoystick>(true);
            //添加事件
            move.onMoveStart.AddListener(OnMoveStart);
            move.onMove.AddListener(OnMove);
            move.onMoveEnd.AddListener(OnMoveEnd);
        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(myRay.origin.x, myRay.origin.y), Vector2.down, 0.05f);
                if (hit.collider)
                {
                    //如果是任务给与者
                    if (hit.collider.GetComponent<QuestGiver>())
                    {
                        DialogueManager.I.StartQuestGiverDialogue(hit.collider.GetComponent<QuestGiver>());
                        return;
                    }
                    if (hit.collider.GetComponent<Talker>())
                    {
                        //如果是商人
                        if (hit.collider.GetComponent<Talker>().IsVendor)
                        {
                            UI.UIManager.I.TogglePanel<ShopPanel>(true, null, FindObjectOfType<UIRootManager>().transform.FindChildByName("HeroCanvas"));
                        }
                        else
                            DialogueManager.I.StartNormalTalkerDialogue(hit.collider.GetComponent<Talker>());
                        return;
                    }
                }
            }
        }
        /// <summary>
        ///  停止移动时切换到idle动画
        /// </summary>
        private void OnMoveEnd()
        {
            anim.speed = 0;
            anim.SetTrigger(lastDir.ToString());
            anim.SetBool(AnimConst.run, false);
        }

        /// <summary>
        ///  移动时播放run的动画
        /// </summary>
        /// <param name="pos"></param>
        private void OnMove(Vector2 pos)
        {
            //获取摇杆偏移量
            float joyPositionX = pos.x;
            float joyPositionY = pos.y;
            float currentAngle = CharacterAnimStateSwitch.CaculaterAngle(joyPositionX, joyPositionY);
            if (currentAngle < 45f && currentAngle >= 0f || currentAngle < 360f && currentAngle >= 315f)//左
            {
                anim.SetTrigger(AnimConst.left);
                move.axisY.speed = 0;
                move.axisX.speed = status.moveSpeed * 0.4f;
                lastDir = Direction.Left;
            }
            else if (currentAngle >= 45f && currentAngle < 135f)//上
            {
                anim.SetTrigger(AnimConst.up);
                move.axisX.speed = 0;
                move.axisY.speed = status.moveSpeed * 0.4f;
                lastDir = Direction.Up;
            }
            else if (currentAngle >= 135f && currentAngle < 225f)//右
            {
                anim.SetTrigger(AnimConst.right);
                move.axisY.speed = 0;
                move.axisX.speed = status.moveSpeed * 0.4f;
                lastDir = Direction.Right;
            }
            else if (currentAngle < 315f && currentAngle >= 225f)//下
            {
                anim.SetTrigger(AnimConst.down);
                move.axisX.speed = 0;
                move.axisY.speed = status.moveSpeed * 0.4f;
                lastDir = Direction.Down;
            }
        }

        /// <summary>
        ///  开始移动时切换到run动画
        /// </summary>
        private void OnMoveStart()
        {
            anim.speed = 1;
            anim.SetBool(AnimConst.run, true);
        }
    }
}
