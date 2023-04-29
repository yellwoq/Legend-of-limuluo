using Common;
using Components;
using MVC;
using Player;
using QuestSystem;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Bag
{
    public delegate void ItemInfoListener(string itemID, int amount);
    public delegate void CanGetItemAgain();
    /// <summary>
    /// 背包面板，管理背包里的格子,处理拾取物品并添加到物品栏
    /// </summary>
    public class BagPanel : BasePanel
    {
        private static BagPanel instance;
        public static BagPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType(typeof(BagPanel)) as BagPanel;
                    if (instance != null)
                        instance.InitBag();
                    else
                        //创建脚本对象(说明场景中没有存在的对象) 
                        new GameObject("Singleton of " + typeof(BagPanel)).AddComponent<BagPanel>();//此时立即执行Awake 
                }
                return instance;
            }
        }
        public static BagPanel I
        {
            get { return Instance; }
        }

        private CanvasGroup group;

        /// <summary>
        /// 背包格子集合
        /// </summary>
        [DisplayName("背包格子集合", true)]
        public List<BagItemGrid> itemGridList = new List<BagItemGrid>();
        [DisplayName("武器装备区", true)]
        public BagItemGrid weaponBox;
        [DisplayName("防具装备区", true)]
        public BagItemGrid clothesBox;
        [DisplayName("鞋子装备区", true)]
        public BagItemGrid shoesBox;
        /// <summary>
        /// 背包物品预制体
        /// </summary>
        [HideInInspector]
        public GameObject bagItemPrefab;
        [HideInInspector]
        public GameObject gameItemPrefab;
        [SerializeField, DisplayName("物品丢弃位置", true)]
        public Transform playerDropPosition = null;
        private Transform content;
        [HideInInspector]
        public GameObject gameItemList;
        /// <summary>
        /// 背包物品映射
        /// </summary>
        public Dictionary<int, BagStoreVO> bagMap;
        /// <summary>
        /// 显示背包物品映射
        /// </summary>
        public Dictionary<int, BagItem> bagShowMap;
        /// <summary>
        /// 物品映射
        /// </summary>
        public Dictionary<int, int> Items { get; set; }
        /// <summary>
        /// 装备的物品映射
        /// </summary>
        public Dictionary<string, string> equipMap;
        [DisplayName("是否已满", true)]
        public bool isFull = false;
        public event ItemInfoListener OnGetItemEvent;
        public event ItemInfoListener OnLoseItemEvent;
        public event CanGetItemAgain OnDropFromBag;
        protected override void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }
        public void InitBag()
        {
            bagMap = new Dictionary<int, BagStoreVO>();
            bagShowMap = new Dictionary<int, BagItem>();
            Items = new Dictionary<int, int>();
            group = GetComponent<CanvasGroup>();
            Hide();
            equipMap = new Dictionary<string, string>();
            content = transform.FindChildByName("Content");
            gameItemList = GameObject.Find("GameItemList");
            itemGridList.AddRange(content.GetComponentsInChildren<BagItemGrid>());
            gameItemPrefab = ResourceManager.Load<GameObject>("GameItem");
            bagItemPrefab = ResourceManager.Load<GameObject>("BagItem");
            playerDropPosition = FindObjectOfType<PlayerStatus>(true).transform.FindChildComponentByName<Transform>("DropPosition");
            weaponBox = transform.FindChildComponentByName<BagItemGrid>("WeaponBox");
            clothesBox = transform.FindChildComponentByName<BagItemGrid>("ClothesBox");
            shoesBox = transform.FindChildComponentByName<BagItemGrid>("ShoesBox");
        }
        public override void Show()
        {
            group.alpha = 1;
            group.blocksRaycasts = true;
            ItemPanel.I.isBagShow = true;
        }
        public void ClearData()
        {
            for (int i = 0; i < itemGridList.Count; i++)
            {
                if (itemGridList[i].transform.childCount > 0)
                {
                    Destroy(itemGridList[i].transform.GetChild(0).gameObject);
                }
            }
            bagMap.Clear();
            bagShowMap.Clear();
            Items.Clear();
        }
        public override void Hide()
        {

            group.alpha = 0;
            group.blocksRaycasts = false;
        }

        /// <summary>
        /// 根据id拾取物品，并添加到物品栏里
        /// </summary>
        /// <param name="id">物品的id</param>
        /// <param name="num">拾取的物品数量</param>
        public void GetItem(int id, int num = 1)
        {
            //查找是否已存在该物品
            BagItemGrid grid = null;
            BagItem item = null;
            foreach (BagItemGrid temp in itemGridList)
            {
                item = temp.transform.GetComponentInChildren<BagItem>();
                if (item != null && item.id == id)
                {
                    grid = temp;
                    break;
                }
            }
            //如果存在，则num+1；
            if (grid != null)
            {
                item.PlusNum(num);
                bagShowMap[id] = item;
                SetBagItem(item);
                AddItem(id, num);
            }
            //如果不存在，查找空的方格，然后把新创建的物品放到空格里
            else
            {
                foreach (BagItemGrid temp in itemGridList)
                {
                    item = temp.transform.GetComponentInChildren<BagItem>();
                    //查找到空的格子
                    if (item == null)
                    {
                        grid = temp;
                        break;
                    }
                }
                if (grid != null)
                {
                    GameObject itemGO = grid.gameObject.AddChild(bagItemPrefab);
                    itemGO.transform.localPosition = Vector3.zero; //把添加的物品位置归零
                    itemGO.GetComponent<BagItem>().SetItem(id, num); //将物品显示出来
                    bagShowMap[id] = itemGO.GetComponent<BagItem>();
                    SetBagItem(bagShowMap[id]);
                    AddItem(id, num);
                }
                else
                {
                    //背包已满
                    Debug.Log("背包已满");
                    isFull = true;
                }
            }
        }
        /// <summary>
        /// 添加物品映射
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void AddItem(int itemID, int amount = 1)
        {
            if (Items.ContainsKey(itemID)) Items[itemID] += amount;
            else
            {
                Items.Add(itemID, amount);
            }
            //如果该物品是任务道具，则会调用相应的方法
            OnGetItemEvent?.Invoke(itemID.ToString(), GetItemAmountByID(itemID));
            //更新任务
            QuestManager.Instance.UpdateObjectivesText();
        }
        /// <summary>
        /// 丢弃物品
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        public void DropItem(int id, int num = 1)
        {
            if (num == 0) { return; }
            //查找是否已存在该物品
            BagItemGrid grid = null;
            BagItem item = null;
            foreach (BagItemGrid temp in itemGridList)
            {
                item = temp.transform.GetComponentInChildren<BagItem>();
                if (item != null && item.id == id)
                {
                    grid = temp;
                    break;
                }
            }
            Debug.Log(grid);
            //如果存在，则丢弃
            if (grid != null)
            {
                OnDropFromBag?.Invoke();
                item.SubtractNum(num);
                if (item.num <= 0)
                {
                    //bagMap.Remove(item.id);
                    Destroy(item.gameObject);
                    isFull = false;
                }
                LoseItem(id, num);
            }
            else
            {
                Alert.Show("丢弃失败", "没有该物品");
            }
        }
        /// <summary>
        /// 丢弃物品，更新物品映射表
        /// </summary>
        /// <param name="item"></param>
        public void LoseItem(int itemID, int amount = 1)
        {
            //如果没有该物品
            if (!HasItemWithID(itemID.ToString())) return;
            //如果有任务需要这么多的物品或者本身没有该物品
            if (itemID == 0 || QuestManager.Instance.HasQuestRequiredItem(itemID.ToString(), GetItemAmountByID(itemID) - amount) || GetItemAmountByID(itemID) < 1) return;
            Items[itemID] -= amount;
            if (Items[itemID] <= 0) Items.Remove(itemID);
            OnLoseItemEvent?.Invoke(itemID.ToString(), GetItemAmountByID(itemID));
            QuestManager.Instance.UpdateObjectivesText();
        }
        /// <summary>
        /// 获取物品数量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetItemAmountByID(int id)
        {
            if (Items.ContainsKey(id))
            {
                return Items[id];
            }
            return 0;
        }
        /// <summary>
        /// 根据Id判断是否有该物品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasItemWithID(string id)
        {
            return GetItemAmountByID(int.Parse(id)) > 0;
        }
        /// <summary>
        /// 根据物品设置存储数据
        /// </summary>
        /// <param name="item"></param>
        public void SetBagItem(BagItem item)
        {
            BagStoreVO bagStoreVO = new BagStoreVO();
            bagStoreVO.id = item.id;
            bagStoreVO.number = item.num;
            Enum.TryParse(ItemInfoManager.I.GetObjectInfoById(item.id).bigType, out bagStoreVO.bigItemType);
            Enum.TryParse(ItemInfoManager.I.GetObjectInfoById(item.id).type, out bagStoreVO.itemType);
            bagStoreVO.heroType = (ApplyHeroType)ItemInfoManager.I.GetObjectInfoById(item.id).applyHeroID;
            bagMap[item.id] = bagStoreVO;
        }
        /// <summary>
        /// 获取物品信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BagStoreVO GetBagStoreItem(int id)
        {
            return bagMap[id] == null ? bagMap[id] : null;
        }
        /// <summary>
        /// 判定是否有某个任务需要某数量的某个道具
        /// </summary>
        /// <param name="itemID">要判定的道具</param>
        /// <param name="leftAmount">要判定的数量</param>
        /// <returns>是否需要该道具</returns>
        public bool IsQuestRequiredItem(Quest quest, string itemID, int leftAmount)
        {
            //入伍按顺序完成
            if (quest.CmpltObjectiveInOrder)
            {
                foreach (Objective o in quest.Objectives)
                {
                    //当目标是收集类目标且在提交任务同时会失去相应道具时，才进行判断
                    if (o is CollectObjective co && itemID == co.ItemID && co.LoseItemAtSubmit)
                    {
                        if (o.IsComplete && o.InOrder)
                        {
                            //如果剩余的道具数量不足以维持该目标完成状态
                            if (o.Amount > leftAmount)
                            {
                                Objective tempObj = o.NextObjective;
                                while (tempObj != null)
                                {
                                    //则判断是否有后置目标在进行，以保证在打破该目标的完成状态时，后置目标不受影响
                                    if (tempObj.CurrentAmount > 0 && tempObj.OrderIndex > o.OrderIndex)
                                    {
                                        return true;
                                    }
                                    tempObj = tempObj.NextObjective;
                                }
                            }
                            return false;
                        }
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
