using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action ItemBeginDrag;
    public event Action<Vector2> ItemDragging;
    public event Action ItemDropped;

    public void OnBeginDrag(PointerEventData eventData)
    {
        ItemBeginDrag?.Invoke();
    }

    // rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    public void OnDrag(PointerEventData eventData)
    {
        ItemDragging?.Invoke(eventData.delta);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Вызываем событие с экранными координатами
        ItemDropped?.Invoke();
    }
}
