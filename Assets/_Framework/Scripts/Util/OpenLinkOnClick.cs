using UnityEngine;
using UnityEngine.EventSystems;

namespace _Framework.Scripts.Util
{
    public class OpenLinkOnClick : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private string link;

        public void OnPointerDown(PointerEventData eventData)
        {
            Application.OpenURL(link);
        }
    }
}