using Common;
using MVC;
using UnityEngine;

namespace Bag
{
    /// <summary>
    /// 在游戏中展示的游戏物品
    /// </summary>
    [System.Serializable]
    public class GameItem : MonoBehaviour
    {
        public string id;
        public int num;
        public string itemName;
        public Sprite itemIcon;
        public string description;
        public BigItemType bigItemType;
        public DetailItemType detailType;

        public GameItem(string id, int num = 1)
        {
            SetGameItem(id, num);
        }
        public GameItem()
        {

        }
        public void SetGameItem(string id, int num = 1)
        {
            this.id = id;
            this.num = num;
            BagItemVO bagItemVO = ItemInfoManager.I.GetObjectInfoById(int.Parse(id));
            Texture2D iconTexture = ResourceManager.Load<Texture2D>(bagItemVO.icon_name);
            GetComponent<SpriteRenderer>().sprite = Sprite.Create(iconTexture, new Rect(0, 0, iconTexture.width, iconTexture.height), new Vector2(0.5f, 0.5f));
            itemName = bagItemVO.name;
            bigItemType = (BigItemType)System.Enum.Parse(typeof(BigItemType), bagItemVO.bigType);
            detailType = (DetailItemType)System.Enum.Parse(typeof(DetailItemType), bagItemVO.type);
            description = bagItemVO.description;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                BagPanel.Instance.GetItem(int.Parse(id), num);
                if (!BagPanel.Instance.isFull)
                    Destroy(gameObject);
            }
        }
    }
}
/// <summary>
/// 可用接口
/// </summary>
public interface IUsable
{
    void OnUse();
}
