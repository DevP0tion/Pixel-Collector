using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PixelCollector.Util.UI
{
    public class AutoSelectOnHover: MonoBehaviour, IPointerEnterHandler
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            button.Select();
        }
    }
}