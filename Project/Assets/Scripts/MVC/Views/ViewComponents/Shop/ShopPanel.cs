using Bag;
using Common;
using UI;
using UnityEngine;

namespace MVC
{
    /// <summary>
    /// 商店界面
    /// </summary>
    public class ShopPanel : BasePanel
    {
        [SerializeField, DisplayName("物品预制体")]
        private GameObject CreateItem;
        [SerializeField, DisplayName("存放物品格子预制体")]
        private GameObject bagItemGridPrefabs;
        [SerializeField, DisplayName("商店存放物品父物体")]
        private GameObject storeParentList;
        [SerializeField, DisplayName("背包存放物品父物体")]
        private GameObject myBagParentList;
        [SerializeField]
        private CanvasGroup gameCanvas;
        private void Start()
        {
            InitShop();
        }
        public override void Show()
        {
            gameCanvas.alpha = 0;
            gameCanvas.blocksRaycasts = false;
            base.Show();
        }
        public void InitShop()
        {
            foreach (var storeItem in ItemInfoManager.I.objectInfoDict)
            {
                if (storeItem.Value.bigType == "MissionProp") continue;
                GameObject storeItemGrid = GameObjectPool.I.CreateObject("StoreItemGrid", bagItemGridPrefabs, storeParentList.transform.FindChildByName("Content"));
                GameObject currentstoreItem = GameObjectPool.I.CreateObject("StoreItem", CreateItem, storeItemGrid.transform);
                currentstoreItem.GetComponent<StoreItem>().id = storeItem.Key;
                currentstoreItem.GetComponent<StoreItem>().numText.gameObject.SetActive(false);
                currentstoreItem.GetComponent<StoreItem>().myItemPanelType = ItemInPanelType.good;
                currentstoreItem.GetComponent<StoreItem>().SetUI();
            }
            foreach (var myBagitem in BagPanel.I.Items)
            {
                if (ItemInfoManager.I.GetObjectInfoById(myBagitem.Key).bigType == "MissionProp") continue;
                GameObject myBagItemGrid = GameObjectPool.I.CreateObject("MyBagItemGrid", bagItemGridPrefabs, myBagParentList.transform.FindChildByName("Content"));
                GameObject currentBagItem = GameObjectPool.I.CreateObject("MyBagItem", CreateItem, myBagItemGrid.transform);
                currentBagItem.GetComponent<StoreItem>().id = myBagitem.Key;
                currentBagItem.GetComponent<StoreItem>().num = myBagitem.Value;
                currentBagItem.GetComponent<StoreItem>().myItemPanelType = ItemInPanelType.bagGood;
                currentBagItem.GetComponent<StoreItem>().SetUI();
            }
        }
        /// <summary>
        /// 背包信息更新
        /// </summary>
        public void UpDateMyBag()
        {
            Transform contentParent = myBagParentList.transform.FindChildByName("Content");
            for (int i = 0; i < contentParent.childCount; i++)
            {
                Transform itemTrans = contentParent.GetChild(i);
                GameObjectPool.I.CollectObject(itemTrans.GetChild(0).gameObject);
                GameObjectPool.I.CollectObject(itemTrans.gameObject);
                
            }
            foreach (var myBagitem in BagPanel.I.Items)
            {
                if (ItemInfoManager.I.GetObjectInfoById(myBagitem.Key).bigType == "MissionProp") continue;
                GameObject myBagItemGrid = GameObjectPool.I.CreateObject("MyBagItemGrid", bagItemGridPrefabs, myBagParentList.transform.FindChildByName("Content"));
                GameObject currentBagItem = GameObjectPool.I.CreateObject("MyBagItem", CreateItem, myBagItemGrid.transform);
                currentBagItem.GetComponent<StoreItem>().id = myBagitem.Key;
                currentBagItem.GetComponent<StoreItem>().num = myBagitem.Value;
                currentBagItem.GetComponent<StoreItem>().myItemPanelType = ItemInPanelType.bagGood;
                currentBagItem.GetComponent<StoreItem>().SetUI();
            }
        }
        public override void Hide()
        {
            ShopItemPanel.I.Hide();
            gameCanvas.alpha = 1;
            gameCanvas.blocksRaycasts = true;
            base.Hide();
        }
    }
}
