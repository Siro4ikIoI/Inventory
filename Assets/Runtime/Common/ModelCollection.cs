using System;
using System.Collections.Generic;

public class ModelCollection
{
    private Dictionary<InventoryType, InventoryModel> _inventories = new();
    public InventoryModel GetInventory(InventoryType type)
    {
        return _inventories[type];
    }

    public void AddInventory(InventoryType type, InventoryModel inventory)
    {
        _inventories.Add(type, inventory);
    }

    public IEnumerable<InventoryModel> GetAllInventories()
    {
        return _inventories.Values;
    }

    private DragAndDropContainer _dragAndDropContainer = null;

    public void SetDragAndDropContainer(DragAndDropContainer dragAndDropContainer)
    {
        _dragAndDropContainer = dragAndDropContainer;
    }

    public DragAndDropContainer GetDragAndDrop()
    {
        return _dragAndDropContainer;
    }
}
