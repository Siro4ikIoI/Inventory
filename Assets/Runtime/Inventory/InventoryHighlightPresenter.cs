public class InventoryHighlightPresenter
{
    private ModelCollection _modelCollection;

    private DragAndDropModel _dragAndDropModel;

    public InventoryHighlightPresenter(ModelCollection modelCollection)
    {
        _modelCollection = modelCollection;
    }

    private void OnDragAndDropChanged(DragAndDropModel dragAndDropModel)
    {
        if (dragAndDropModel == null)
        {
            _dragAndDropModel.Item.Rotated -= OnItemRotated;
            _dragAndDropModel.EndDraged -= OnEndDrag;
            _dragAndDropModel.InventorySetted -= OnInventorySetted;

            _dragAndDropModel = null;
        }
        else
        {
            _dragAndDropModel = dragAndDropModel;

            _dragAndDropModel.InventorySetted += OnInventorySetted;
            _dragAndDropModel.EndDraged += OnEndDrag;
            _dragAndDropModel.Item.Rotated += OnItemRotated;
        }
    }

    private void OnInventorySetted()
    {
        HighlightInventoryCells();
    }

    private void OnEndDrag()
    {
        ResetAllHighlights();
    }

    private void OnItemRotated(Direction DirectionObject)
    {
        HighlightInventoryCells();
    }

    private void ResetAllHighlights()
    {
        Item emptyItem = new Item(-1, ItemType.GRANADE, new int[1, 1] { { 0 } });
        foreach (var inventory in _modelCollection.GetAllInventories())
        {
            inventory.CanAddItem(emptyItem, new Pair(0, 0));
        }
    }

    private void HighlightInventoryCells()
    {
        InventoryType inventoryType = _dragAndDropModel.CurrentInventory;
        if (inventoryType == InventoryType.NONE)
        {
            ResetAllHighlights();
            return;
        }

        Inventory inventory = _modelCollection.GetInventory(inventoryType);
        Pair itemPosition = _dragAndDropModel.CurrentPosition;

        inventory.CanAddItem(_dragAndDropModel.Item, itemPosition);
    }

    public void Enable()
    {
        _modelCollection.ChangedDragAndDrop += OnDragAndDropChanged;
    }

    public void Disable()
    {
        _modelCollection.ChangedDragAndDrop -= OnDragAndDropChanged;
    }
}
