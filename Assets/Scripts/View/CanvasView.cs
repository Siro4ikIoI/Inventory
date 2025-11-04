using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasView : MonoBehaviour
{
    // События для взаимодействия с презентером
    public event Action RequestItemsGeneration;
    public event Func<Guid, InventoryView, float, float, bool> ItemDroped;

    [SerializeField] private ItemView itemPrefab;
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private InventoryView caseView;

    private Dictionary<Guid, ItemView> spawnedItems = new Dictionary<Guid, ItemView>();
    private Dictionary<Guid, InventoryView> itemContainers = new Dictionary<Guid, InventoryView>();

    void Start()
    {
        inventoryView.SetShape(5, 9);
        caseView.SetShape(7, 4);

        // Вызываем событие для генерации набора предметов
        RequestItemsGeneration?.Invoke();       
    }

    // Метод для создания итемов в кейсе (вызывается презентером)
    public void SpawnItems(List<ItemData> itemsData)
    {
        if (caseView == null)
        {
            Debug.LogError("CaseView не установлен!");
            return;
        }

        foreach (var itemData in itemsData)
        {
            SpawnItem(itemData, caseView);
        }
    }

    private void SpawnItem(ItemData itemData, InventoryView container)
    {
        if (itemPrefab == null || container == null)
        {
            Debug.LogError("ItemPrefab или Container не установлены!");
            return;
        }

        ItemView itemView = Instantiate<ItemView>(itemPrefab, container.GetContainer());

        Vector2 tablePosition = new Vector2(
            itemData.Position.x, 
            Math.Abs(itemData.Position.y - container.Row + 1)
        );

        Vector2 localPosition = container.GetLocalPosition(tablePosition);

        itemView.Initialize(itemData);
        itemView.SetPosition(localPosition);
        itemView.ItemDropped += OnItemDropped;
        spawnedItems[itemData.Id] = itemView;
        itemContainers[itemData.Id] = container;       
    }

    // Обработчик события drop от ItemView
    private void OnItemDropped(ItemView item, Vector2 screenPosition)
    {
        InventoryView currentContainer = itemContainers.ContainsKey(item.Id) ? itemContainers[item.Id] : null;
        InventoryView newContainer = null;
        Vector2 tablePosition = Vector2.zero;



        // Определяем, в какую область попал итем
        if (inventoryView != null && inventoryView.IsPointInside(screenPosition))
        {
            inventoryView.GetTablePosition(screenPosition, out tablePosition);
            newContainer = inventoryView;
        }
        else if (caseView != null && caseView.IsPointInside(screenPosition))
        {
            caseView.GetTablePosition(screenPosition, out tablePosition);
            newContainer = caseView;
        }

        // Если итем не попал ни в одну область, возвращаем на исходную позицию
        if (newContainer == null)
        {
            item.ResetPosition();
            return;
        }

        int matrixX = (int)tablePosition.x;
        int matrixY = (int)Math.Abs(tablePosition.y - newContainer.Row + 1);

        // Запрашиваем у презентера разрешение на размещение
        bool canPlace = ItemDroped?.Invoke(item.Id, newContainer, matrixX, matrixY) ?? false;

        if (canPlace)
        {
            // Перемещаем итем в новый контейнер, если он изменился
            if (currentContainer != newContainer)
            {
                item.transform.SetParent(newContainer.GetContainer(), true);
                itemContainers[item.Id] = newContainer;
            }

            // Устанавливаем новую позицию
            item.SetPosition(newContainer.GetLocalPosition(tablePosition));
        }
        else
        {
            // Возвращаем на исходную позицию
            item.ResetPosition();
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        foreach (var item in spawnedItems.Values)
        {
            if (item != null)
            {
                item.ItemDropped -= OnItemDropped;
            }
        }
    }
}
