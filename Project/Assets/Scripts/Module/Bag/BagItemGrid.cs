using MVC;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Bag
{
    /// <summary>
    /// 背包格子，管理物品放置及交换
    /// </summary>
    public class BagItemGrid : BaseMono, IDropHandler
    {
        /// <summary>
        /// 物品被放置的时候
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrop(PointerEventData eventData)
        {
            //被拖动的物体
            GameObject item = eventData.pointerDrag;
            //被拖动的物体只能是Item，否则不作相应
            if (item.gameObject.tag != Common.Tags.Bag_item.ToString())
            {
                return;
            }
            BagItem dragItem = item.GetComponent<BagItem>();
            GameObject box = eventData.pointerCurrentRaycast.gameObject;
            BagItemVO currentDropItemVO = ItemInfoManager.I.GetObjectInfoById(dragItem.id);
            PlayerPrefs.SetInt(KeyList.CURRENT_ITEM_ID, dragItem.id);
            ItemPanel.I.Init();
            if (box.tag == Common.Tags.Bag_item.ToString())
            {
                BagItem collisionBagItem = box.GetComponent<BagItem>();
                //如果是装备栏里面的物品
                if (box.transform.parent == BagPanel.I.weaponBox.transform ||
                    box.transform.parent == BagPanel.I.clothesBox.transform ||
                    box.transform.parent == BagPanel.I.shoesBox.transform)
                {
                    if (ItemPanel.I.currentUserHeroVO.heroType == ItemInfoManager.I.GetObjectInfoById(collisionBagItem.id).bigType)
                        ItemPanel.I.ChangeEquip(box.transform.parent);
                    return;
                }
                //如果原来是装备栏物品
                if (dragItem.parent == BagPanel.I.weaponBox.transform ||
                  dragItem.parent == BagPanel.I.clothesBox.transform ||
                   dragItem.parent == BagPanel.I.shoesBox.transform)
                {
                    //如果是同一类物体
                    if (collisionBagItem.id == dragItem.id)
                    {
                        ItemPanel.I.UnEquip(dragItem);
                        Destroy(item);
                        return;
                    }
                    //如果不是同一类物品
                    else
                    {
                        if (ItemPanel.I.currentUserHeroVO.heroType == ItemInfoManager.I.GetObjectInfoById(collisionBagItem.id).bigType)
                        {
                            ItemPanel.I.ChangeEquip(box.transform.parent);
                        }
                        return;
                    }
                }
                //交换位置
                Transform temp = dragItem.parent;
                Debug.Log(temp + "---" + box.transform.parent);
                item.transform.SetParent(box.transform.parent);
                box.transform.SetParent(temp);
                box.transform.localPosition = Vector3.zero;
                item.transform.localPosition = Vector3.zero;
            }
            else if (box.tag == Common.Tags.Bag_item_grid.ToString())
            {
                switch (box.name)
                {
                    case "WeaponBox":
                        if (currentDropItemVO.type != DetailItemType.Weapon.ToString() || box.transform.childCount >= 1)
                        {
                            item.transform.localPosition = Vector3.zero;
                            return;
                        }
                        break;
                    case "ClothesBox":
                        if (currentDropItemVO.type != DetailItemType.Clothes.ToString() || box.transform.childCount >= 1)
                        {
                            item.transform.localPosition = Vector3.zero;
                            return;
                        }
                        break;
                    case "ShoesBox":
                        if (currentDropItemVO.type != DetailItemType.Shoes.ToString() || box.transform.childCount >= 1)
                        {
                            item.transform.localPosition = Vector3.zero;
                            return;
                        }
                        break;
                }
                //如果当前是装备栏
                if (transform == BagPanel.I.weaponBox.transform ||
                    transform == BagPanel.I.clothesBox.transform ||
                    transform == BagPanel.I.shoesBox.transform)
                {
                    //如果原来是装备栏中的
                    if (dragItem.parent == transform)
                    {
                        ItemPanel.I.UnEquip(dragItem);
                        return;
                    }
                    else
                    {
                        ItemPanel.I.isDragItemMode = true;
                        ItemPanel.I.EquipItemWithNotJudge();
                        ItemPanel.I.isDragItemMode = false;
                        return;
                    }
                }
                //别拖动到空格子，设置位置
                dragItem.transform.SetParent(transform);
                item.transform.localPosition = Vector3.zero;
            }

        }
    }
}