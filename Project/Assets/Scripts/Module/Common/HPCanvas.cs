using UnityEngine;
using UnityEngine.UI;

namespace Common
{
    /// <summary>
    ///  血条
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class HPCanvas : MonoBehaviour
    {
        [SerializeField, DisplayName("血条")]
        public Slider slider;
        private CharacterStatus character;
        private void Awake()
        {
            GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
            GetComponent<Canvas>().worldCamera = Camera.main;
            slider = transform.GetComponentInChildren<Slider>();
            character = transform.GetComponentInParent<CharacterStatus>();
        }

        private void Update()
        {
            // 更新血值
            slider.value = character.currentHP;
            // 朝向相机
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
