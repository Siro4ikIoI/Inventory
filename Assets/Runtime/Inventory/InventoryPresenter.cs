using System;
using UnityEngine;

public class InventoryPresenter
{
    private Inventory _inventory;
    private InventoryView _inventoryView;

    private ModelCollection _modelCollection;
    private ItemSettings _itemSettings;

    private DragAndDropModel _dragAndDropModel;

    public InventoryPresenter(Inventory inventory, InventoryView inventoryView, 
                                ModelCollection modelCollection, ItemSettings itemSettings)
    {
        _inventory = inventory;
        _inventoryView = inventoryView;

        _modelCollection = modelCollection;
        _itemSettings = itemSettings;
    }

    private void OnItemAdded(Item item, (int row, int col) position)
    {
        ItemView itemprefab = _itemSettings.GetItemSoByType(item.Type).item;
        ItemView itemView = GameObject.Instantiate<ItemView>(itemprefab, _inventoryView.transform);

        Vector2 tablePosition = new Vector2(position.col, Math.Abs(position.row - _inventory.Shape.row + 1));
        itemView.SetPosition(_inventoryView.GetLocalPosition(tablePosition));
        itemView.SetRotation(item.GetRotation(), item.ToMatrix().GetStructure());

        ItemPresenter itemPresenter = new ItemPresenter(item, itemView, _modelCollection);
        itemPresenter.Enable();
    }

    private void OnCollisionWhenAdding(int[,] collisionMatrix)
    {
        _inventoryView.HighlightCells(collisionMatrix);
    }

    public void Enable()
    {
        _inventory.ItemAdded += OnItemAdded;
        _inventory.CollisionWhenAdding += OnCollisionWhenAdding;

        _modelCollection.ChangedDragAndDrop += OnDragAndDropChanged;
    }

    public void Disable()
    {
        _inventory.CollisionWhenAdding -= OnCollisionWhenAdding;
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

        InventoryType inventoryType = _inventory.Type;

        _inventoryView.GetTablePosition(position, out Vector2 tablePosition);
        int col = (int)tablePosition.x;
        int row = (int)(_inventory.Shape.row - tablePosition.y - 1);
        (int row, int col) cellPosition = new (row, col);

        _dragAndDropModel.SetInventoryAndPosition(inventoryType, cellPosition);
    }
}
