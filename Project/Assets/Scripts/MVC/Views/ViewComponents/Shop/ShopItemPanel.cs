using Bag;
using Common;
using Components;
using Player;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    /// 商店物品显示
    /// </summary>
    public class ShopItemPanel : BasePanel, IDragHandler
    {
        #region 属性
        private static ShopItemPanel instance;
        public static ShopItemPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    //在场景中查找已经存在的对象(说明客户端代码在Awake中调用Instance)
                    instance = FindObjectOfType(typeof(ShopItemPanel)) as ShopItemPanel;
                    if (instance != null)
                        instance.Initialize();
                    else
                        //创建脚本对象(说明场景中没有存在的对象) 
                        new GameObject("Singleton of " + typeof(ItemPanel)).AddComponent<ItemPanel>();//此时立即执行Awake 
                }
                return instance;
            }
        }
        [DisplayName("当前物品ID",true)]
        public int itemId = 0;
        [DisplayName("当前处理数量", true)]
        public int handleNum = 0;
        [DisplayName("本物品所有的总数量", true)]
        public int numCount = 0;
        [DisplayName("当前操作类型", true, true, "购买", "售卖")]
        public ShopHandleType currentHandleType;
        [HideInInspector]
        public StoreItem currentItem;
        private BagItemVO currentItemVO;
        public static ShopItemPanel I
        {
            get { return Instance; }
        }
        #endregion

        #region UI属性相关
        [SerializeField, DisplayName("物品名字显示文本")]
        private Text itemName;
        [SerializeField, DisplayName("物品描述信息显示文本")]
        private Text des;
        [SerializeField, DisplayName("物品属性值显示文本")]
        private Text applyValue;
        [SerializeField, DisplayName("物品数量显示文本")]
        private Text num;
        [SerializeField, DisplayName("物品价格显示文本")]
        private Text price;
        [SerializeField, DisplayName("操作按钮")]
        private Button handleButton;
        [SerializeField, DisplayName("数量递增按钮")]
        private Button addButton;
        [SerializeField, DisplayName("数量递减按钮")]
        private Button subButton;
        [SerializeField, DisplayName("出售时的物品数量滚动条")]
        private Slider numSlider;
        #endregion
        protected virtual void Initialize()
        {
            instance= this;
        }
        protected override void Awake()
        {
            base.Awake();
            numSlider.onValueChanged.AddListener(ChangeHandleNumber);
            addButton.onClick.AddListener(() =>
            {
                handleNum++;
                numSlider.value = handleNum;
            });
            subButton.onClick.AddListener(() =>
            {
                handleNum--;
                if (handleNum < 0) handleNum = 0;
                numSlider.value = handleNum;
            });
        }
        public override void Show()
        {
            Init();
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            switch (currentHandleType)
            {
                case ShopHandleType.Buy:
                    handleButton.onClick.AddListener(BuyItem);
                    break;
                case ShopHandleType.Sell:
                    handleButton.onClick.AddListener(SellItem);
                    break;
            }
            UpDateItem();
        }
        public override void Hide()
        {
            numSlider.value = 0;
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            switch (currentHandleType)
            {
                case ShopHandleType.Buy:
                    handleButton.onClick.RemoveListener(BuyItem);
                    break;
                case ShopHandleType.Sell:
                    handleButton.onClick.RemoveListener(SellItem);
                    break;
            }
        }
        void Init()
        {
            if (PlayerPrefs.HasKey(KeyList.CURRENT_STOREITEM_ID))
            {
                itemId = PlayerPrefs.GetInt(KeyList.CURRENT_STOREITEM_ID);
            }
            currentItemVO = ItemInfoManager.I.GetObjectInfoById(itemId);
            Debug.Log(currentItemVO.price_buy);
            if (currentHandleType==ShopHandleType.Sell)
            numCount = currentItem.num;
        }
        /// <summary>
        /// 购买物品
        /// </summary>
        public void BuyItem()
        {
            if (handleNum < 1) { Alert.Show("购买失败", "还没有确定购买的数量"); return; }
            //金钱不足，购买失败
            int currentMoney = GameController.I.crtHero.money;
            //金钱足够，购买成功
            if (currentMoney >= currentItemVO.price_buy * handleNum)
            {
                UserHeroVO currentUHVO = GameController.I.crtHero;
                currentUHVO.money -= currentItemVO.price_buy * handleNum;
                currentUHVO.tipMessage = "物品购买成功";
                Alert.Show("购买成功", currentUHVO.tipMessage);
                GameController.I.crtHero = currentUHVO;
                currentItem.num += handleNum;
                BagPanel.I.GetItem(itemId, handleNum);
                FindObjectOfType<ShopPanel>().UpDateMyBag();
                Hide();
            }
            else
            {
                Alert.Show("购买失败", "金钱不足");
            }

        }
        /// <summary>
        /// 出售物品
        /// </summary>
        public void SellItem()
        {
            if (handleNum < 1) { Alert.Show("出售失败", "还没有确定出售的数量"); return; }
            UserHeroVO currentUHVO = GameController.I.crtHero;
            currentUHVO.money += currentItemVO.price_sell * handleNum;
            currentUHVO.tipMessage = "物品出售成功";
            Alert.Show("出售成功", currentUHVO.tipMessage);
            GameController.I.crtHero = currentUHVO;
            currentItem.num -= handleNum;
            BagPanel.I.DropItem(itemId, handleNum);
            JudgeIsNeedDestroy();
            FindObjectOfType<ShopPanel>().UpDateMyBag();
            Hide();
        }

        private void ChangeHandleNumber(float changeValue)
        {
            numSlider.value = changeValue;
            handleNum = (int)changeValue;
            num.text = "数量：" + handleNum;
            if (currentHandleType == ShopHandleType.Sell)
            {
                numSlider.maxValue = numCount;
                price.text = "出售价格：" + (currentItemVO.price_sell * handleNum).ToString();
            }
            else
            {
                numSlider.maxValue = 99;
                price.text = "购买价格：" + (currentItemVO.price_buy * handleNum).ToString();
            }
        }
        /// <summary>
        /// 判断其是否需要销毁
        /// </summary>
        public void JudgeIsNeedDestroy()
        {
            //如果数量为0，销毁
            if (currentItem.num <= 0)
            {
                GameObjectPool.I.CollectObject(currentItem.gameObject);
                GameObjectPool.I.CollectObject(currentItem.transform.parent.gameObject);
                BagPanel.Instance.bagShowMap.Remove(itemId);
            }
            Hide();
        }
        /// <summary>
        /// 更新信息
        /// </summary>
        public void UpDateItem()
        {
            if (PlayerPrefs.HasKey(KeyList.CURRENT_STOREITEM_ID))
            {
                itemId = PlayerPrefs.GetInt(KeyList.CURRENT_STOREITEM_ID);
            }
            BagItemVO bagItemVO = ItemInfoManager.I.GetObjectInfoById(itemId);
            if (bagItemVO != null)
            {
                itemName.text = bagItemVO.name;
                des.text = bagItemVO.description;
                DetailItemType itemType = 0;
                string attributeDes = null;
                System.Enum.TryParse(bagItemVO.type, out itemType);
                Text handleTxt = handleButton.GetComponentInChildren<Text>();
                switch (itemType)
                {
                    case DetailItemType.HpDrug:
                        attributeDes = "血量+";
                        break;
                    case DetailItemType.MpDrug:
                        attributeDes = "魔量+";
                        break;
                    case DetailItemType.Weapon:
                        attributeDes = "力量+";
                        break;
                    case DetailItemType.Clothes:
                        attributeDes = "体力+";
                        break;
                    case DetailItemType.Shoes:
                        attributeDes = "移速+";
                        break;
                }
                applyValue.text = "属性：" + attributeDes + bagItemVO.applyValue.ToString();
                switch (currentHandleType)
                {
                    case ShopHandleType.Buy:
                        handleTxt.text = "购买";
                        break;
                    case ShopHandleType.Sell:
                        handleTxt.text = "出售";
                        break;
                }
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Vector3.Lerp(transform.position, eventData.position, 0.5f);
            if (Vector3.Distance(transform.position, eventData.position) <= 0.1f)
            {
                transform.position = eventData.position;
            }
            Debug.Log(eventData.position);
        }
    }
    public enum ShopHandleType
    {
        Buy,
        Sell
    }
}
