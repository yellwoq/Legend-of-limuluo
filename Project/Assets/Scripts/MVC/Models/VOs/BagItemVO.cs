using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVC
{
    /// <summary>
    ///  物品数据
    /// </summary>
    public class BagItemVO 
    {
        public int id;
        public string name;
        public string icon_name;
        public string bigType;
        public string type;
        public int price_buy;
        public int price_sell;
        public int applyValue;
        public int applyHeroID;
        public string description;    
    }
}
