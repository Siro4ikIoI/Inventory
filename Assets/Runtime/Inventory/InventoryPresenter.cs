using System;
using UnityEngine;

public class InventoryPresenter
{
    private Inventory _inventory;
    private InventoryView _inventoryView;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    private DragAndDropModel _dragAndDropModel;

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

        _modelCollection.ChangedDragAndDrop += OnDragAndDropChanged;
    }

    public void Disable()
    {
        _inventory.CollisionWhenAdding -= OnCollisionWhenAdding;
        _inventory.ItemExtracted -= OnItemExtracted;
        _inventory.ItemAdded -= OnItemAdded;

        _modelCollection.ChangedDragAndDrop -= OnDragAndDropChanged;
        if (_dragAndDropModel != null)
        {
            _dragAndDropModel.Draged -= OnDragged;
            _dragAndDropModel = null;
        }
    }

    private void OnDragAndDropChanged(DragAndDropModel dragAndDropModel)
    {
        if (_dragAndDropModel != null)
        {
            _dragAndDropModel.Draged -= OnDragged;
        }

        _dragAndDropModel = dragAndDropModel;

        if (_dragAndDropModel != null)
        {
            _dragAndDropModel.Draged += OnDragged;
        }
    }

    private void OnDragged(float x, float y)
    {
        Vector3 position = new Vector3(x, y, 0f);

        if (!_inventoryView.IsPointInside(position))
            return;

        InventoryType inventoryType = InventoryType.NONE;
        foreach (var pair in _canvasView.GetAllInventoryViews())
        {
            if (pair.Value == _inventoryView)
            {
                inventoryType = pair.Key;
                break;
            }
        }

        if (inventoryType == InventoryType.NONE)
            return;

        _inventoryView.GetTablePosition(position, out Vector2 tablePosition);
        int col = (int)tablePosition.x;
        int row = (int)(_inventory.Shape.Row - tablePosition.y - 1);
        Pair cellPosition = new Pair(row, col);

        _dragAndDropModel.SetInventoryAndPosition(inventoryType, cellPosition);
    }
}
