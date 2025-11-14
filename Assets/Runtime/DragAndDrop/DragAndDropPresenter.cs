using System;
using UnityEngine;

public class DragAndDropPresenter
{
    DragAndDropModel _dragAndDropModel;
    DragAndDropView _dragAndDropView;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public DragAndDropPresenter(DragAndDropModel dragAndDropModel, DragAndDropView dragAndDropView,
                                    ModelCollection modelCollection, CanvasView canvasView)
    {
        _dragAndDropModel = dragAndDropModel;
        _dragAndDropView = dragAndDropView;

        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnBeginDrag()
    {
        if (_modelCollection.GetCurrentDragAndDrop() != null)
            return;

        Inventory sourceInventory = null;
        foreach (Inventory inventory in _modelCollection.GetAllInventories())
        {
            if (inventory.ContainsItem(_dragAndDropModel.Item))
            {
                sourceInventory = inventory;
            }
        }

        if (sourceInventory == null)
            return;

        sourceInventory.TryExtractItem(_dragAndDropModel.Item, out Pair position);
        _dragAndDropModel.PreviousItemInventory = sourceInventory;
        _dragAndDropModel.PreviousItemPosition = position;
        _dragAndDropModel.PreviousItemRotation = _dragAndDropModel.Item.GetRotation();

        _modelCollection.SetCurrentDragAndDrop(_dragAndDropModel);
    }

    private void OnDrag(Vector2 delta)
    {
        if (_modelCollection.GetCurrentDragAndDrop() != _dragAndDropModel)
            return;

        ItemView itemView = _canvasView.GetItemView(_dragAndDropModel.Item.Id);
        itemView.SetPosition(itemView.GetPosition() + delta / _canvasView.Canvas.scaleFactor);

        _dragAndDropModel.CurrentInventory = GetInventoryAtScreenPosition(itemView.transform.position);
        Pair itemPosition = new Pair(-1, -1);
        if (_dragAndDropModel.CurrentInventory != InventoryType.NONE)
        {
            Inventory inventory = _modelCollection.GetInventory(_dragAndDropModel.CurrentInventory);
            InventoryView inventoryView = _canvasView.GetInventoryView(_dragAndDropModel.CurrentInventory);
            itemPosition = GetItemPosition(itemView.transform.position, inventory, inventoryView);
        }

        _dragAndDropModel.CurrentPosition = itemPosition;
        _dragAndDropModel.Drag();
    }

    private void OnEndDrag() 
    {
        if (_modelCollection.GetCurrentDragAndDrop() != _dragAndDropModel)
            return;

        InventoryType inventoryType = _dragAndDropModel.CurrentInventory;
        Pair itemPosition = _dragAndDropModel.CurrentPosition;

        if (inventoryType == InventoryType.NONE 
            || !_modelCollection.GetInventory(inventoryType).TryAddItem(_dragAndDropModel.Item, itemPosition))
        {
            _dragAndDropModel.Item.SetRotation(_dragAndDropModel.PreviousItemRotation);
            _dragAndDropModel.PreviousItemInventory.TryAddItem(_dragAndDropModel.Item, _dragAndDropModel.PreviousItemPosition);
        }

        _dragAndDropModel.EndDrag();

        _dragAndDropModel.PreviousItemInventory = null;
        _dragAndDropModel.PreviousItemPosition = new Pair(-1, -1);
        _dragAndDropModel.PreviousItemRotation = Direction.N;
        
        _modelCollection.SetCurrentDragAndDrop(null);
    }

    private InventoryType GetInventoryAtScreenPosition(Vector2 screenPosition)
    {
        InventoryType inventoryType = InventoryType.NONE;

        foreach (var inventoryPair in _canvasView.GetAllInventoryViews())
        {
            if (inventoryPair.Value.IsPointInside(screenPosition))
            {
                inventoryType = inventoryPair.Key;
            }
        }

        return inventoryType;
    }

    private Pair GetItemPosition(Vector2 position, Inventory inventory, InventoryView inventoryView)
    {
        inventoryView.GetTablePosition(position, out Vector2 tablePosition);
        int col = (int)tablePosition.x;
        int row = (int)(inventory.Shape.Row - tablePosition.y - 1);
        return new Pair(row, col);
    }

    public void Enable()
    {
        _dragAndDropView.ItemBeginDrag += OnBeginDrag;
        _dragAndDropView.ItemDragging += OnDrag;
        _dragAndDropView.ItemDropped += OnEndDrag;
    }

    public void Disable()
    {
        _dragAndDropView.ItemDropped -= OnEndDrag;
        _dragAndDropView.ItemDragging -= OnDrag;
        _dragAndDropView.ItemBeginDrag -= OnBeginDrag;
    }
}
