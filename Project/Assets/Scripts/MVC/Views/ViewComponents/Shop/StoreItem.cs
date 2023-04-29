using Bag;
using Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    /// 商店显示物品
    /// </summary>
    public class StoreItem : MonoBehaviour, IPointerClickHandler
    {
        [DisplayName("物品编号", true)]
        public int id;
        [DisplayName("物品数量", true)]
        public int num;
        [DisplayName("所代表的物品类型", true, true, "商品", "背包物品")]
        public ItemInPanelType myItemPanelType;
        [SerializeField, DisplayName("物品图像显示")]
        private Image icon;
        public Text numText;
        public void SetUI()
        {
            Texture2D iconTexture = ResourceManager.Load<Texture2D>(ItemInfoManager.I.GetObjectInfoById(id).icon_name);
            icon.sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
            numText.text = num.ToString();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            PlayerPrefs.SetInt(KeyList.CURRENT_STOREITEM_ID, id);
            if (myItemPanelType == ItemInPanelType.good)
                ShopItemPanel.I.currentHandleType = ShopHandleType.Buy;
            else
                ShopItemPanel.I.currentHandleType = ShopHandleType.Sell;
            ShopItemPanel.I.currentItem = this;
            ShopItemPanel.I.Show();
        }
    }
    public enum ItemInPanelType
    {
        good,
        bagGood
    }
}