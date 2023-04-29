using System;
using Common;
using Player;
using UnityEngine;

namespace MapSystem
{
    /// <summary>
    /// 地图开关
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public class MapSwitch : MonoBehaviour
    {
        [DisplayName("目标关闭的地图")]
        public MapAgent[] targetCloseMap;
        [DisplayName("目标打开的地图")]
        public MapAgent[] targetOpenMap;
        [SerializeField,DisplayName("是否需要传送位置")]
        private bool isNeedSendPos=false;
        [SerializeField,ConditionalHide("传送位置", "isNeedSendPos",true)]
        private Vector3 sendPos;
        private void Awake()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                foreach (var closeMap in targetCloseMap)
                {
                    closeMap.gameObject.SetActive(false);
                    closeMap.SetPeople(false);
                }
                foreach (var openMap in targetOpenMap)
                {
                    openMap.gameObject.SetActive(true);
                    if(openMap.MapName!=null)
                    MapAgentManager.I.mapTipText.text = openMap.MapName;
                    openMap.SetPeople(true);
                }
                if (isNeedSendPos)
                {
                    SendPos(PlayerManager.I.playerTrans, sendPos);
                }
            }
        }

        private void SendPos(Transform playerTrans, Vector3 sendPos)
        {
            playerTrans.position = sendPos;
        }
    }
}
