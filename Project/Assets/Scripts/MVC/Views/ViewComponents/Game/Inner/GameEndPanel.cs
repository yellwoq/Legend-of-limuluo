using UI;
using UnityEngine;

namespace MVC
{

    public class GameEndPanel : BasePanel
    {
        private CanvasGroup canvasGroup;
        protected override void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public override void Show()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1;

        }
        public override void Hide()
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
            SendNotification(NotiList.LOAD_MAINSCENE);
        }
    }
}
