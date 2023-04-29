using System.Collections.Generic;
using UnityEngine;

namespace MVC
{
    /// <summary>
    ///  处理物品数据: 获取装备
    /// </summary>
    public class BagProxy : BaseProxy
    {
        /// <summary>
        ///  NAME
        /// </summary>
        public new const string NAME = "BagProxy";
        /// <summary>
        ///  构造函数
        /// </summary>
        public BagProxy()
        {
            this.ProxyName = NAME;
        }
        /// <summary>
        ///  获取物品表
        /// </summary>
        public void GetItemMap()
        {
            Debug.Log("GetItem!");
            // 打开数据库
            OpenDB();
            // 读取整张表
            string selectStr = "select * from ItemInfo";
            List<Dictionary<string, object>> result = db.ExecuteQuery(selectStr);
            // 如果有数据
            if (result != null)
            {
                Dictionary<int, BagItemVO> itemsMap = new Dictionary<int, BagItemVO>();
                for (int i = 0; i < result.Count; i++)
                {
                    BagItemVO item = new BagItemVO();
                    // 读取字段
                    item.id =int.Parse(result[i]["ID"].ToString());
                    item.name = result[i]["Name"].ToString();
                    item.icon_name = result[i]["Icon_Name"].ToString();
                    item.bigType = result[i]["BigType"].ToString();
                    item.type = result[i]["Type"].ToString();
                    item.price_buy = int.Parse(result[i]["Price_Buy"].ToString());
                    item.price_sell = int.Parse(result[i]["Price_Sell"].ToString());
                    item.applyValue = int.Parse(result[i]["ApplyValue"].ToString());
                    item.applyHeroID = int.Parse(result[i]["ApplyHeroID"].ToString());
                    item.description = result[i]["Des"].ToString();
                    // add
                    itemsMap.Add(item.id, item);
                }
                #region 另外的方法
                //// 遍历
                //while (reader.Read())
                //{
                //    BagItemVO item = new BagItemVO();
                //    // 读取字段
                //    item.id = (int)reader.GetValue(reader.GetOrdinal("ID"));
                //    item.name = reader.GetValue(reader.GetOrdinal("Name")).ToString();
                //    item.icon_name = reader.GetValue(reader.GetOrdinal("Icon_Name")).ToString();
                //    item.bigType = reader.GetValue(reader.GetOrdinal("BigType")).ToString();
                //    item.type = reader.GetValue(reader.GetOrdinal("Type")).ToString();
                //    item.price_buy = (int)reader.GetValue(reader.GetOrdinal("Price_Buy"));
                //    item.price_sell = (int)reader.GetValue(reader.GetOrdinal("Price_Sell"));
                //    item.applyValue = (int)reader.GetValue(reader.GetOrdinal("ApplyValue"));
                //    item.applyHeroID = (int)reader.GetValue(reader.GetOrdinal("ApplyHeroID"));
                //    item.description = reader.GetValue(reader.GetOrdinal("Des")).ToString();
                //    // add
                //    itemsMap.Add(item.id, item);
                //} 
                #endregion
                // 发送 获取装备 成功
                SendNotification(NotiList.GET_ITEM_MAP + NotiList.SUCCESS, itemsMap);
            }
            else // 没有数据
            {
                SendNotification(NotiList.GET_ITEM_MAP + NotiList.FAILURE, "没有物品数据!!!");
            }
            // 关闭数据库
            CloseDB();
        }
    }
}
