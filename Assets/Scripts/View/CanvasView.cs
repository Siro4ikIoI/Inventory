using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasView : MonoBehaviour
{
    // События для взаимодействия с презентером
    public event Func<int, InventoryType, int, int, bool> ItemDroped;

    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private InventoryView caseView;

    private Dictionary<InventoryType, InventoryView> inventories = new();
    private Dictionary<int, ItemView> spawnedItems = new Dictionary<int, ItemView>();
    private Dictionary<int, InventoryView> itemContainers = new Dictionary<int, InventoryView>();

    private void Awake()
    {
        inventories.Add(InventoryType.CASE, caseView);
        inventories.Add(InventoryType.INVENTARY, inventoryView);
    }

    public void InitializeInventory(InventoryType type, int row, int col)
    {
        inventories[type].SetShape(row, col);
    }

    public void SpawnItem(int id, ItemView prefab, int row, int col)
    {
        InventoryView @case = inventories[InventoryType.CASE];

        if (prefab == null || @case == null)
        {
            Debug.LogError("ItemPrefab или Container не установлены!");
            return;
        }

        ItemView itemView = Instantiate<ItemView>(prefab, @case.GetContainer());
        Vector2 tablePosition = new Vector2(col, Math.Abs(row - @case.Row + 1));
        Vector2 localPosition = @case.GetLocalPosition(tablePosition);

        itemView.Initialize(id);
        itemView.SetPosition(localPosition);
        itemView.ItemDropped += OnItemDropped;
        spawnedItems[id] = itemView;
        itemContainers[id] = @case;       
    }

    // Обработчик события drop от ItemView
    private void OnItemDropped(ItemView item, Vector2 screenPosition)
    {
        InventoryType newContainerType = InventoryType.CASE;
        InventoryView newContainer = null;
        Vector2 tablePosition = Vector2.zero;

        foreach (var inventoryPair in inventories)
        {
            // Определяем, в какую область попал итем
            if (inventoryPair.Value.IsPointInside(screenPosition))
            {
                inventoryPair.Value.GetTablePosition(screenPosition, out tablePosition);
                newContainer = inventoryPair.Value;
                newContainerType = inventoryPair.Key;
            }
        }

        // Если итем не попал ни в одну область, возвращаем на исходную позицию
        if (newContainer == null)
        {
            item.ResetPosition();
            return;
        }

        int col = (int)tablePosition.x;
        int row = (int)Math.Abs(tablePosition.y - newContainer.Row + 1);

        // Запрашиваем у презентера разрешение на размещение
        bool canPlace = ItemDroped?.Invoke(item.Id, newContainerType, row, col) ?? false;

        if (canPlace)
        {
            // Перемещаем итем в новый контейнер, если он изменился
            item.transform.SetParent(newContainer.GetContainer(), true);
            itemContainers[item.Id] = newContainer;

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
