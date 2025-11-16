using System;
using UnityEngine;

public class DragAndDropPresenter
{
    DragAndDropModel _dragAndDropModel;
    DragAndDropView _dragAndDropView;

    private ModelCollection _modelCollection;

    public DragAndDropPresenter(DragAndDropModel dragAndDropModel, DragAndDropView dragAndDropView,
                                    ModelCollection modelCollection)
    {
        _dragAndDropModel = dragAndDropModel;
        _dragAndDropView = dragAndDropView;

        _modelCollection = modelCollection;
    }

    private void OnBeginDrag()
    {
        if (_modelCollection.GetCurrentDragAndDrop() != null)
            return;

        _dragAndDropModel.SetInventoryAndPosition(InventoryType.NONE, new (-1, -1));

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

        sourceInventory.HideItem(_dragAndDropModel.Item);
        _dragAndDropView.transform.SetParent(_dragAndDropView.transform.parent.parent);
        _dragAndDropModel.PreviousItemInventory = sourceInventory;

        _modelCollection.SetCurrentDragAndDrop(_dragAndDropModel);
    }

    private void OnDrag(Vector2 delta)
    {
        if (_modelCollection.GetCurrentDragAndDrop() != _dragAndDropModel)
            return;

        RectTransform rectTransform = (RectTransform)_dragAndDropView.transform;
        rectTransform.anchoredPosition += delta / rectTransform.parent.localScale.x;

        _dragAndDropModel.SetInventoryAndPosition(InventoryType.NONE, new (-1, -1));

        Vector3 worldPosition = rectTransform.position;
        _dragAndDropModel.Drag(worldPosition.x, worldPosition.y);
    }

    private void OnEndDrag() 
    {
        if (_modelCollection.GetCurrentDragAndDrop() != _dragAndDropModel)
            return;

        InventoryType inventoryType = _dragAndDropModel.CurrentInventory;
        (int row, int col) itemPosition = _dragAndDropModel.CurrentPosition;

        if (inventoryType != InventoryType.NONE 
            && _modelCollection.GetInventory(inventoryType).CanAddItem(_dragAndDropModel.Item, itemPosition))
        {
            _dragAndDropModel.PreviousItemInventory.TryExtractItem(_dragAndDropModel.Item, out _);
            _modelCollection.GetInventory(inventoryType).TryAddItem(_dragAndDropModel.Item, itemPosition);
        }
        else
        {
            _dragAndDropModel.PreviousItemInventory.RestoreItem(_dragAndDropModel.Item);
        }

        _dragAndDropModel.EndDrag();
        _modelCollection.SetCurrentDragAndDrop(null);

        _dragAndDropModel.Destroy();
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
