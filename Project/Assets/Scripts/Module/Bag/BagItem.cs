using Common;
using MVC;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bag
{
    /// <summary>
    /// 背包物品，管理物品拖拽及物品信息显示
    /// </summary>
    [System.Serializable]
    public class BagItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [DisplayName("数量",true)]
        public int num = 0;
        [DisplayName("物品编号", true)]
        public int id;
        private BagItemVO bagItemInfo;
        [HideInInspector]
        public Transform parent;
        [HideInInspector]
        private Transform grandParent;
        private Text numTxt;
        private Image icon;
        public Image Icon
        {
            get
            {
                if (icon == null)
                    icon = transform.FindChildComponentByName<Image>("ItemImage");
                return icon;
            }
        }

        #region 物品数据处理逻辑
        /// <summary>
        /// 通过Id在格子里添加物品
        /// </summary>
        /// <param name="id">物品id</param>
        /// <param name="num">数量</param>
        public void SetItem(int id, int num = 1)
        {
            this.id = id;
            bagItemInfo = ItemInfoManager.I.GetObjectInfoById(id);
            icon = transform.FindChildComponentByName<Image>("ItemImage");
            Texture2D iconTexture = ResourceManager.Load<Texture2D>(bagItemInfo.icon_name);
            icon.sprite =Sprite.Create(iconTexture,new Rect(0,0, iconTexture.width,iconTexture.height),new Vector2(0.5f,0.5f)) ;
            numTxt = transform.FindChildComponentByName<Text>("Number");
            this.num = num;
            numTxt.text = num.ToString();
        }

        /// <summary>
        /// 添加物品数量
        /// </summary>
        /// <param name="num">添加的数量</param>
        public void PlusNum(int num = 1)
        {
            this.num += num;
            numTxt = transform.FindChildComponentByName<Text>("Number");
            numTxt.text = this.num.ToString();
        }
        /// <summary>
        /// 减少物品数量
        /// </summary>
        /// <param name="num"></param>
        public void SubtractNum(int num = 1)
        {
            this.num -= num;
            if (num < 0)
            {
                GUIStyle style = new GUIStyle();
                style.fontSize = 20;
                GUILayout.Label("数量不足", style);
                return;
            }
            numTxt = transform.FindChildComponentByName<Text>("Number");
            numTxt.text = this.num.ToString();
        }
        /// <summary>
        /// 清空格子存的物品信息
        /// </summary>
        public void CleanrInfo()
        {
            id = 0;
            bagItemInfo = null;
            num = 0;
        }
        #endregion

        #region 物品移动，拖拽等相关处理逻辑
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = eventData.position;
            // 取消相对应的射线检测，拖动结束才能检测到底部格子而不被遮挡
            transform.GetComponent<Image>().raycastTarget = false;
            transform.SetParent(grandParent);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            parent = transform.parent;
            if (ItemPanel.I.isBagShow)
            {
                grandParent = BagPanel.I.transform;
            }
            else
            {
                switch (ItemPanel.I.openType)
                {
                    case OpenWindowType.DialogueDescription:
                        grandParent = DialogueSystem.DialogueManager.I.DescriptionWindow.transform;
                        break;
                    case OpenWindowType.QuestDescription:
                        grandParent = QuestSystem.QuestManager.I.DescriptionWindow.transform;
                        break;
                }
            }
            
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (transform.parent == grandParent)
            {
                transform.SetParent(parent);
                transform.localPosition = Vector3.zero;
            }
            transform.GetComponent<Image>().raycastTarget = true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PlayerPrefs.SetInt(KeyList.CURRENT_ITEM_ID, id);
            Transform itemPanel=null;
            if (ItemPanel.I.isBagShow)
            {
                BagPanel bagPanel = FindObjectOfType<BagPanel>();
                itemPanel = bagPanel.transform.FindChildByName("ItemPanel");
            }
            else
            {
                switch (ItemPanel.I.openType)
                {
                    case OpenWindowType.DialogueDescription:
                        itemPanel = DialogueSystem.DialogueManager.I.DescriptionWindow.transform.FindChildByName("ItemPanel");
                        break;
                    case OpenWindowType.QuestDescription:
                        itemPanel = QuestSystem.QuestManager.I.DescriptionWindow.transform.FindChildByName("ItemPanel");
                        break;
                }
                
            }
            Debug.Log(itemPanel);
            itemPanel.SetParent(gameObject.transform);
            if (transform.parent.rectTransform().localPosition.y <= -470)
            {
                itemPanel.localPosition = Vector3.zero + new Vector3(-236, -50, 0);
            }
            else
            {
                itemPanel.localPosition = Vector3.zero + new Vector3(0, -225, 0);
            }
            itemPanel.GetComponent<ItemPanel>().UpItemUI();
            if (ItemPanel.I.isBagShow)
            {
                BagPanel bagPanel = FindObjectOfType<BagPanel>();
                itemPanel.SetParent(bagPanel.transform);
            }
            else
            {
                switch (ItemPanel.I.openType)
                {
                    case OpenWindowType.DialogueDescription:
                        itemPanel.SetParent(DialogueSystem.DialogueManager.I.DescriptionWindow.transform);
                        break;
                    case OpenWindowType.QuestDescription:
                        itemPanel.SetParent(QuestSystem.QuestManager.I.DescriptionWindow.transform);
                        break;
                }
                
            }
            itemPanel.GetComponent<ItemPanel>().Show();
        }

        #endregion
    }
}
