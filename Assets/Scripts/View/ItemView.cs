using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public event Action<ItemView> ItemBeginDrag;
    public event Action<ItemView, Vector2> ItemDragging;    
    public event Action<ItemView, Vector2> ItemDropped;

    public int Id { get; private set; }

    [SerializeField] private Image iconImage;
    [SerializeField] private Text nameText;

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Canvas canvas;
    private Transform originParent;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    // Инициализация итема данными
    public void Initialize(int id)
    {
        Id = id;
    }

    // Установка позиции итема
    public void SetPosition(Vector2 position)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
            originalPosition = position;
            originParent = transform.parent;
        }
    }

    // Возврат на исходную позицию
    public void ResetPosition()
    {
        if (rectTransform != null)
        {
            transform.SetParent(originParent);
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        transform.SetParent(canvas.transform);
        ItemBeginDrag?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform != null && canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
            ItemDragging?.Invoke(this, transform.position);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Вызываем событие с экранными координатами
        ItemDropped?.Invoke(this, transform.position);
    }
}
