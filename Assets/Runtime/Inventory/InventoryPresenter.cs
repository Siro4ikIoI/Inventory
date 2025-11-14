using System;
using UnityEngine;

public class InventoryPresenter
{
    private Inventory _inventory;
    private InventoryView _inventoryView;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public InventoryPresenter(Inventory inventory, InventoryView inventoryView, 
                                ModelCollection modelCollection, CanvasView canvasView)
    {
        _inventory = inventory;
        _inventoryView = inventoryView;

        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnItemAdded(Item item, Pair position)
    {
        ItemView itemView = _canvasView.GetItemView(item.Id);

        itemView.transform.SetParent(_inventoryView.GetContainer(), true);

        Vector2 tablePosition = new Vector2(position.Col, Math.Abs(position.Row - _inventory.Shape.Row + 1));
        itemView.SetPosition(_inventoryView.GetLocalPosition(tablePosition));
    }

    private void OnItemExtracted(Item item)
    {
        ItemView itemView = _canvasView.GetItemView(item.Id);

        itemView.transform.SetParent(_canvasView.transform);
    }

    private void OnCollisionWhenAdding(int[,] collisionMatrix)
    {
        _inventoryView.HighlightCells(collisionMatrix);
    }

    public void Enable()
    {
        _inventory.ItemAdded += OnItemAdded;
        _inventory.ItemExtracted += OnItemExtracted;
        _inventory.CollisionWhenAdding += OnCollisionWhenAdding;
    }

    public void Disable()
    {
        _inventory.CollisionWhenAdding -= OnCollisionWhenAdding;
        _inventory.ItemExtracted -= OnItemExtracted;
        _inventory.ItemAdded -= OnItemAdded;
    }
}
