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

        _dragAndDropModel.SetInventoryAndPosition(InventoryType.NONE, new Pair(-1, -1));

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

        RectTransform rectTransform = (RectTransform)_dragAndDropView.transform;
        rectTransform.anchoredPosition += delta / _canvasView.Canvas.scaleFactor;

        _dragAndDropModel.SetInventoryAndPosition(InventoryType.NONE, new Pair(-1, -1));

        Vector3 worldPosition = rectTransform.position;
        _dragAndDropModel.Drag(worldPosition.x, worldPosition.y);
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
