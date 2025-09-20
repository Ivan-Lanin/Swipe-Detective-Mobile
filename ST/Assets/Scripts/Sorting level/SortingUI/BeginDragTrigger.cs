using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BeginDragTrigger : MonoBehaviour, IBeginDragHandler, IPointerDownHandler
{
    public UnityEvent onBeginDrag;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDrag?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onBeginDrag?.Invoke();
    }
}
