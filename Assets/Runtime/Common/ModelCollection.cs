using System;
using System.Collections.Generic;

public class ModelCollection
{
    public event Action<DragAndDropModel> ChangedDragAndDrop;

    private Dictionary<InventoryType, Inventory> _inventories = new();
    public Inventory GetInventory(InventoryType type)
    {
        return _inventories[type];
    }

    public void AddInventory(InventoryType type, Inventory inventory)
    {
        _inventories.Add(type, inventory);
    }

    public IEnumerable<Inventory> GetAllInventories()
    {
        return _inventories.Values;
    }

    private DragAndDropModel _currentDragAndDrop = null;

    public void SetCurrentDragAndDrop(DragAndDropModel dragAndDropModel)
    {
        _currentDragAndDrop = dragAndDropModel;
        ChangedDragAndDrop?.Invoke(_currentDragAndDrop);
    }

    public DragAndDropModel GetCurrentDragAndDrop()
    {
        return _currentDragAndDrop;
    }
}
