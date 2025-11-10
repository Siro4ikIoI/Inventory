using System;
using System.Collections.Generic;
using UnityEngine;

public class CanvasView : MonoBehaviour
{
    // События для взаимодействия с презентером
    public event Func<int, InventoryType, int, int, bool> ItemDroped;
    public event Action<int, InventoryType, int, int> ItemDragPositionChanged;
    public event Action<int> ItemBeginDrag;
    public event Action ItemRotation;

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
        itemView.ItemBeginDrag += OnItemBeginDrag;
        itemView.ItemDropped += OnItemDropped;
        itemView.ItemDragging += OnItemDragging;
        spawnedItems[id] = itemView;
        itemContainers[id] = @case;       
    }

    private void OnItemBeginDrag(ItemView item)
    {
        ResetAllHighlights();
        ItemBeginDrag?.Invoke(item.Id);
    }

    private InventoryType GetInventoryAtScreenPosition(Vector2 screenPosition)
    {
        InventoryType inventoryType = InventoryType.NONE;

        foreach (var inventoryPair in inventories)
        {
            if (inventoryPair.Value.IsPointInside(screenPosition))
            {
                inventoryType = inventoryPair.Key;
            }
        }

        return inventoryType;
    }

    private void OnItemDragging(ItemView item, Vector2 screenPosition)
    {
        InventoryType targetInventoryType = GetInventoryAtScreenPosition(screenPosition);

        if (targetInventoryType != InventoryType.NONE)
        {
            inventories[targetInventoryType].GetTablePosition(screenPosition, out Vector2 tablePosition);
            int col = (int)tablePosition.x;
            int row = (int)Math.Abs(tablePosition.y - inventories[targetInventoryType].Row + 1);

            ItemDragPositionChanged?.Invoke(item.Id, targetInventoryType, row, col);
        }
        else
        {
            ResetAllHighlights();
        }
    }

    // Обработчик события drop от ItemView
    private void OnItemDropped(ItemView item, Vector2 screenPosition)
    {
        Vector2 tablePosition = Vector2.zero;
        InventoryType newContainerType = GetInventoryAtScreenPosition(screenPosition);
        inventories.TryGetValue(newContainerType, out InventoryView newContainer);
       
        int row = -1, col = -1;
        if (newContainerType != InventoryType.NONE)
        {
            newContainer.GetTablePosition(screenPosition, out tablePosition);
            col = (int)tablePosition.x;
            row = (int)Math.Abs(tablePosition.y - newContainer.Row + 1);
        }

        // Запрашиваем у презентера разрешение на размещение
        bool canPlace = ItemDroped?.Invoke(item.Id, newContainerType, row, col) ?? false;

        if (canPlace)
        {
            item.transform.SetParent(newContainer.GetContainer(), true);
            itemContainers[item.Id] = newContainer;

            item.SetPosition(newContainer.GetLocalPosition(tablePosition));
        }
        else
        {
            item.ResetPosition();
        }

        ResetAllHighlights();
    }

    public void HighlightInventoryCells(InventoryType inventoryType, int[,] highlightArray)
    {
        if (inventories.ContainsKey(inventoryType))
        {
            inventories[inventoryType].HighlightCells(highlightArray);
        }
    }

    public void ResetAllHighlights()
    {
        foreach (var inventory in inventories.Values)
        {
            inventory.ResetHighlight();
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий
        foreach (var item in spawnedItems.Values)
        {
            if (item != null)
            {
                item.ItemBeginDrag -= OnItemBeginDrag;
                item.ItemDropped -= OnItemDropped;
                item.ItemDragging -= OnItemDragging;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            ItemRotation?.Invoke();
        }
    }

    public void RotateItem(int id, Direction direction, int[,] blocks)
    {
        ItemView itemView = spawnedItems[id];
        itemView.SetRotation(direction, blocks);
        OnItemDragging(itemView, itemView.transform.position);
    }
}
