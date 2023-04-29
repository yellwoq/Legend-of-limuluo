using System;
using UnityEngine;
using UnityEngine.UI;
using Components;
using Common;
using UI;

namespace Player
{
    /// <summary>
    ///  小地图
    /// </summary>
    public class MiniMap :BasePanel,IView
    {
        private Button mapBtn;

        private new void Awake()
        {
            mapBtn = GetComponent<Button>();
            mapBtn.onClick.AddListener(Hide);
        }

        public override void Hide()
        {
            this.gameObject.SetActive(false);
            transform.parent.FindChildByName("MapButton").gameObject.SetActive(true);
        }
    }
}
