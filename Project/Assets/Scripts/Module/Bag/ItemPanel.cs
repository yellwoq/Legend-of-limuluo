using Common;
using Components;
using MVC;
using Player;
using System.Text;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Bag
{
    /// <summary>
    /// 物品的使用及丢弃
    /// </summary>
    public class ItemPanel : BasePanel, IDragHandler
    {
        private static ItemPanel instance;
        public static ItemPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    //在场景中查找已经存在的对象(说明客户端代码在Awake中调用Instance)
                    instance = FindObjectOfType(typeof(ItemPanel)) as ItemPanel;
                    if (instance != null)
                        instance.Initialize();
                    else
                        //创建脚本对象(说明场景中没有存在的对象) 
                        new GameObject("Singleton of " + typeof(ItemPanel)).AddComponent<ItemPanel>();//此时立即执行Awake 
                }
                return instance;
            }
        }
        //对象被创建时执行
        protected override void Awake()
        {
            Initialize();
        }
        public static ItemPanel I
        {
            get { return Instance; }
        }
        protected virtual void Initialize()
        {
            instance = this;
            base.Awake();
            itemName = transform.FindChildComponentByName<Text>("ItemName");
            des = transform.FindChildComponentByName<Text>("Des");
            applyValue = transform.FindChildComponentByName<Text>("ApplyValue");
            useBtn = transform.FindChildComponentByName<Button>("useBtn");
            dropBtn = transform.FindChildComponentByName<Button>("dropBtn");
            checkBtn = transform.FindChildComponentByName<Button>("checkBtn");
            cancelBtn = transform.FindChildComponentByName<Button>("cancelBtn");
            numSlider = transform.FindChildComponentByName<Slider>("numSlider");
            num = transform.FindChildComponentByName<Text>("num");
            addButton = transform.FindChildComponentByName<Button>("addButton");
            subButton = transform.FindChildComponentByName<Button>("subButton");
            useBtn.onClick.AddListener(TryUseItem);
            dropBtn.onClick.AddListener(TryDropItem);
        }
        private Text itemName, des, applyValue, num;
        private Button useBtn, dropBtn, checkBtn, cancelBtn, closeBtn;
        private Slider numSlider;
        [ReadOnly]
        private int itemId = 0;
        [DisplayName("当前处理数量", true)]
        public int handleNum = 0;
        [DisplayName("本物品所有的总数量", true)]
        public int numCount = 0;
        [HideInInspector]
        public BagItem currentItem;
        public BagItemVO currentBagItemVO;
        public BagStoreVO currentBagStoreVO;
        [HideInInspector]
        public UserHeroVO currentUserHeroVO;
        [DisplayName("正在进行的操作类型", true, true, "使用", "丢弃", "装备")]
        public OperationType operationType;
        [DisplayName("是否是背包中物品显示", true)]
        public bool isBagShow = true;
        [DisplayName("打开的窗口类型", true, true, "对话", "任务")]
        public OpenWindowType openType;
        [DisplayName("是否为拖拽模式", false)]
        public bool isDragItemMode = false;
        private Button addButton;
        private Button subButton;
        public override void Show()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            BagPanel.I.OnDropFromBag += CreateGameItem;
            Init();
        }

        private void CreateGameItem()
        {
            GameObject itemGo = Instantiate(BagPanel.I.gameItemPrefab, BagPanel.I.playerDropPosition.position, Quaternion.identity);
            itemGo.transform.SetParent(BagPanel.I.gameItemList.transform);
            itemGo.GetComponent<GameItem>().SetGameItem(itemId.ToString(), handleNum);
        }

        public override void Hide()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            BagPanel.I.OnDropFromBag -= CreateGameItem;
            ShowUsePanel(false);
        }
        private void ChangeUseNumber(float changeValue)
        {
            numSlider.maxValue = numCount;
            numSlider.value = changeValue;
            handleNum = (int)changeValue;
            num.text = "数量：" + handleNum;
        }
        /// <summary>
        /// 丢弃物品
        /// </summary>
        private void DropItem()
        {
            Debug.Log(handleNum);
            BagPanel.Instance.DropItem(itemId, handleNum);
            //隐藏
            JudgeIsNeedDestroy();
        }
        /// <summary>
        /// 取消
        /// </summary>
        public void Cancel()
        {
            ShowUsePanel(false);

        }
        private void TryUseItem()
        {
            numCount = 1;

            if (currentBagStoreVO.bigItemType == BigItemType.Consumables)
                operationType = OperationType.Use;
            else
                operationType = OperationType.Equip;
            Relatedbinding();
        }
        /// <summary>
        /// 尝试丢弃该物品
        /// </summary>
        public void TryDropItem()
        {
            numCount = currentItem.num;
            operationType = OperationType.Drop;
            Relatedbinding();

        }

        private void Relatedbinding()
        {
            numSlider.onValueChanged.AddListener(ChangeUseNumber);
            ShowUsePanel(true);
            checkBtn.onClick.AddListener(HandleItem);
            cancelBtn.onClick.AddListener(Cancel);
        }

        private void HandleItem()
        {
            switch (operationType)
            {
                case OperationType.Use:
                    UseItem();
                    break;
                case OperationType.Drop:
                    DropItem();
                    break;
                case OperationType.Equip:
                    EquipItem();
                    break;
            }
        }

        /// <summary>
        /// 装备的卸载与装备
        /// </summary>
        public void EquipItem()
        {
            if (numSlider.value < 1)
            {
                return;
            }
            EquipItemWithNotJudge();
        }

        public void EquipItemWithNotJudge()
        {
            int changeValue = currentBagItemVO.applyValue;
            DetailItemType itemType = currentBagStoreVO.itemType;
            ApplyHeroType heroType = currentBagStoreVO.heroType;
            if (heroType.ToString() == currentUserHeroVO.heroType)
            {
                switch (itemType)
                {
                    case DetailItemType.Weapon:
                        JudgeEquip(BagPanel.I.weaponBox.transform, changeValue);
                        break;
                    case DetailItemType.Clothes:
                        JudgeEquip(BagPanel.I.clothesBox.transform, changeValue);
                        break;
                    case DetailItemType.Shoes:
                        JudgeEquip(BagPanel.I.shoesBox.transform, changeValue);
                        break;
                }
                JudgeIsNeedDestroy();
            }
            else
            {
                Alert.Show("装备失败", "英雄类型不符");
                Sound.SoundManager.I.PlaySfx("FaultClick", Sound.SoundManager.ReadAudioClipType.ResourcesLoad);
                return;
            }
        }

        /// <summary>
        /// 装备逻辑判断
        /// </summary>
        /// <param name="equipBox"></param>
        /// <param name="changeValue"></param>
        public void JudgeEquip(Transform equipBox, int changeValue)
        {
            StringBuilder changeName = new StringBuilder();
            switch (equipBox.name)
            {
                case "WeaponBox":
                    changeName.Append("武器");
                    break;
                case "ClothesBox":
                    changeName.Append("防具");
                    break;
                case "ShoesBox":
                    changeName.Append("鞋子");
                    break;
            }
            if (equipBox.childCount <= 0)
            {
                Equip(equipBox);
            }
            else if (equipBox.childCount >= 1)
            {
                if (equipBox.GetComponentInChildren<BagItem>().id != itemId)
                {
                    Alert.Show("替换装备", "已经装备" + changeName.ToString() + ",是否替换？", ChangeEquip, equipBox, true);
                }
                else
                {
                    Alert.Show("卸载装备", "已经装备本道具,是否卸载？", UnEquip, equipBox, true);
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (PlayerPrefs.HasKey(KeyList.CURRENT_ITEM_ID))
            {
                itemId = PlayerPrefs.GetInt(KeyList.CURRENT_ITEM_ID);
            }
            currentBagItemVO = ItemInfoManager.I.GetObjectInfoById(itemId);
            if (!I.isBagShow)
            {
                useBtn.gameObject.SetActive(false);
                dropBtn.gameObject.SetActive(false);
            }
            else
            {
                currentItem = BagPanel.Instance.bagShowMap[itemId];
                currentBagStoreVO = BagPanel.Instance.bagMap[itemId];
                numCount = currentItem.num;
                currentUserHeroVO = GameController.I.crtHero;
                if (currentBagItemVO.bigType == BigItemType.MissionProp.ToString())
                    useBtn.gameObject.SetActive(false);
                else
                    useBtn.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 显示使用面板
        /// </summary>
        /// <param name="isOn"></param>
        private void ShowUsePanel(bool isOn)
        {
            des.gameObject.SetActive(!isOn);
            applyValue.gameObject.SetActive(!isOn);
            if (currentBagItemVO.bigType == BigItemType.MissionProp.ToString())
                useBtn.gameObject.SetActive(false);
            else
                useBtn.gameObject.SetActive(!isOn);
            dropBtn.gameObject.SetActive(!isOn);
            numSlider.gameObject.SetActive(isOn);
            numSlider.value = 0;
            num.gameObject.SetActive(isOn);
            checkBtn.gameObject.SetActive(isOn);
            cancelBtn.gameObject.SetActive(isOn);
            if (isBagShow)
            {
                addButton.gameObject.SetActive(isOn);
                subButton.gameObject.SetActive(isOn);
                if (isOn)
                {
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
                else
                {
                    addButton.onClick.RemoveAllListeners();
                    subButton.onClick.RemoveAllListeners();
                }
            }

        }
        /// <summary>
        /// 使用物品
        /// </summary>
        public void UseItem()
        {
            if (GameController.instance.calculaterTime >= GameController.instance.useItemCDTime)
            {
                if (numSlider.value <= 0)
                {
                    Alert.Show("使用失败", "使用物品数量为0");
                    return;
                }
                int applyValue = currentBagItemVO.applyValue;
                DetailItemType itemType = currentBagStoreVO.itemType;
                if (itemType == DetailItemType.HpDrug)
                {
                    currentUserHeroVO.currentHP += applyValue;
                    if (currentUserHeroVO.currentHP >= currentUserHeroVO.maxHP)
                        currentUserHeroVO.currentHP = currentUserHeroVO.maxHP;
                    PlayerManager.I.playerTrans.GetComponentInChildren<PlayerStatus>().currentHP = currentUserHeroVO.currentHP;
                }
                else if (itemType == DetailItemType.MpDrug)
                {
                    currentUserHeroVO.currentMP += applyValue;
                    if (currentUserHeroVO.currentMP >= currentUserHeroVO.maxMP)
                        currentUserHeroVO.currentMP = currentUserHeroVO.maxMP;
                }
                PlayerManager.I.heroData.SetHeroSave(currentUserHeroVO);
                Sound.SoundManager.I.PlaySfx("UserItem", Sound.SoundManager.ReadAudioClipType.ResourcesLoad);
                //数量减去相应值
                currentItem.SubtractNum(handleNum);
                JudgeIsNeedDestroy();
                GameController.instance.calculaterTime = 0;
            }
        }
        /// <summary>
        /// 切换装备
        /// </summary>
        /// <param name="arg0"></param>
        public void ChangeEquip(object equipBox)
        {

            Transform boxTrans = equipBox as Transform;
            UnEquip(boxTrans);
            Equip(boxTrans);
            currentUserHeroVO.tipMessage = "切换装备成功";
            Alert.Show("装备更新成功", currentUserHeroVO.tipMessage);
            GameController.I.crtHero = currentUserHeroVO;
            currentItem.SubtractNum(1);
            JudgeIsNeedDestroy();
        }
        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="equipBox"></param>
        public void UnEquip(object equipBox)
        {
            BagItem equipBagItem = (equipBox as Transform).GetComponentInChildren<BagItem>();
            UnEquip(equipBagItem);
        }
        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="equipBagItem"></param>
        public void UnEquip(BagItem equipBagItem)
        {
            BagItemVO equipBagItemVO = ItemInfoManager.I.GetObjectInfoById(equipBagItem.id);
            BagPanel.Instance.GetItem(equipBagItem.id, 1);
            switch (equipBagItemVO.type)
            {
                case "Weapon":
                    currentUserHeroVO.force -= equipBagItemVO.applyValue;
                    break;
                case "Clothes":
                    currentUserHeroVO.spirit -= equipBagItemVO.applyValue;
                    break;
                case "Shoes":
                    currentUserHeroVO.speed -= equipBagItemVO.applyValue;
                    break;
            }
            Alert.Show("卸载成功", "原装备已卸载");
            PlayerManager.I.heroData.SetHeroSave(currentUserHeroVO);
            PlayerManager.I.SetPlayerStatus();
            Destroy(equipBagItem.gameObject);
        }
        /// <summary>
        /// 装备
        /// </summary>
        /// <param name="equipBox"></param>
        public void Equip(Transform equipBox)
        {
            GameObject equipObject = Instantiate(BagPanel.Instance.bagItemPrefab, equipBox);
            BagItem equipBagItem = equipObject.GetComponent<BagItem>();
            equipBagItem.SetItem(currentItem.id, 1);
            equipObject.transform.localPosition = Vector3.zero;
            Debug.Log(equipObject.transform.parent);
            switch (currentBagItemVO.type)
            {
                case "Weapon":
                    currentUserHeroVO.force += currentBagItemVO.applyValue;
                    break;
                case "Clothes":
                    currentUserHeroVO.spirit += currentBagItemVO.applyValue;
                    break;
                case "Shoes":
                    currentUserHeroVO.speed += currentBagItemVO.applyValue;
                    break;
            }
            Alert.Show("装备成功", "已经装备该道具");
            if (!isDragItemMode)
            {
                BagPanel.I.OnDropFromBag -= CreateGameItem;
                DropItem();
                BagPanel.I.OnDropFromBag += CreateGameItem;
            }
            else
            {
                currentItem.SubtractNum(1);
            }
            PlayerManager.I.heroData.SetHeroSave(currentUserHeroVO);
            PlayerManager.I.SetPlayerStatus();
            Sound.SoundManager.I.PlaySfx("EquipItem", Sound.SoundManager.ReadAudioClipType.ResourcesLoad);
        }
        /// <summary>
        /// 判断其是否需要销毁
        /// </summary>
        public void JudgeIsNeedDestroy()
        {
            //如果数量为0，销毁
            if (currentItem.num <= 0)
            {
                if(currentBagStoreVO.bigItemType==BigItemType.Consumables)
                Destroy(currentItem.gameObject);
                //BagPanel.Instance.bagShowMap.Remove(itemId);
            }
            ShowUsePanel(false);
            Hide();
        }
        /// <summary>
        /// 更新信息
        /// </summary>
        public void UpItemUI()
        {
            if (PlayerPrefs.HasKey(KeyList.CURRENT_ITEM_ID))
            {
                itemId = PlayerPrefs.GetInt(KeyList.CURRENT_ITEM_ID);
            }
            BagItemVO bagItemVO = ItemInfoManager.I.GetObjectInfoById(itemId);
            if (bagItemVO != null)
            {
                itemName.text = bagItemVO.name;
                des.text = bagItemVO.description;
                DetailItemType itemType = 0;
                string attributeDes = null;
                System.Enum.TryParse(bagItemVO.type, out itemType);
                Text useBtnTxt = useBtn.GetComponentInChildren<Text>();
                switch (itemType)
                {
                    case DetailItemType.HpDrug:
                        attributeDes = "血量+";
                        useBtnTxt.text = "使用";
                        break;
                    case DetailItemType.MpDrug:
                        attributeDes = "魔量+";
                        useBtnTxt.text = "使用";
                        break;
                    case DetailItemType.Weapon:
                        attributeDes = "力量+";
                        if (BagPanel.I.weaponBox.transform.childCount >= 1 && BagPanel.I.weaponBox.transform.GetComponentInChildren<BagItem>().id == itemId)
                            useBtnTxt.text = "卸下";
                        else
                            useBtnTxt.text = "装备";
                        break;
                    case DetailItemType.Clothes:
                        attributeDes = "体力+";
                        if (BagPanel.I.clothesBox.transform.childCount >= 1 && BagPanel.I.clothesBox.transform.GetComponentInChildren<BagItem>().id == itemId)
                            useBtnTxt.text = "卸下";
                        else
                            useBtnTxt.text = "装备";
                        break;
                    case DetailItemType.Shoes:
                        attributeDes = "移速+";
                        if (BagPanel.I.shoesBox.transform.childCount >= 1 && BagPanel.I.shoesBox.transform.GetComponentInChildren<BagItem>().id == itemId)
                            useBtnTxt.text = "卸下";
                        else
                            useBtnTxt.text = "装备";
                        break;
                }

                applyValue.text = "属性：" + attributeDes + bagItemVO.applyValue.ToString();
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
    public enum OperationType
    {
        Use,
        Drop,
        Equip
    }
    public enum OpenWindowType
    {
        DialogueDescription,
        QuestDescription
    }
}
