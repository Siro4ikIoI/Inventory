using System;
using UnityEngine;

public class ItemPresenter
{
    private Item _item;
    private ItemView _itemView;

    private ModelCollection _modelCollection;

    private DragAndDropPresenter _dragAndDropPresenter;
    private DragAndDropModel _dragAndDropModel;

    public ItemPresenter(Item item, ItemView itemView, ModelCollection modelCollection)
    {
        _item = item;
        _itemView = itemView;

        _modelCollection = modelCollection;

        _dragAndDropModel = new DragAndDropModel(item);
        DragAndDropView dragAndDropView = _itemView.DragAndDrop;
        _dragAndDropPresenter = new DragAndDropPresenter(_dragAndDropModel, dragAndDropView, modelCollection);
    }

    private void OnItemRotated(Direction direction)
    {
        _itemView.SetRotation(direction, _item.ToMatrix().GetStructure());
    }

    private void OnDradAndDropModelDestroyed()
    {
        Disable();
        GameObject.Destroy(_itemView.gameObject);
    }

    public void Enable()
    {
        _item.Rotated += OnItemRotated;
        _dragAndDropModel.Destroyed += OnDradAndDropModelDestroyed;
        _dragAndDropPresenter.Enable();
    }

    public void Disable()
    {
        _dragAndDropPresenter.Disable();
        _dragAndDropModel.Destroyed -= OnDradAndDropModelDestroyed;
        _item.Rotated -= OnItemRotated;
    }
}
