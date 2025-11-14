public class InventoryHighlightPresenter
{
    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    private DragAndDropModel _dragAndDropModel;

    public InventoryHighlightPresenter(ModelCollection modelCollection, CanvasView canvasView)
    {
        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnDragAndDropChanged(DragAndDropModel dragAndDropModel)
    {
        if (dragAndDropModel == null)
        {
            _dragAndDropModel.Item.Rotated -= OnItemRotated;
            _dragAndDropModel.EndDraged -= OnEndDrag;
            _dragAndDropModel.Draged -= OnDrag;

            _dragAndDropModel = null;
        }
        else
        {
            _dragAndDropModel = dragAndDropModel;

            _dragAndDropModel.Draged += OnDrag;
            _dragAndDropModel.EndDraged += OnEndDrag;
            _dragAndDropModel.Item.Rotated += OnItemRotated;
        }
    }

    private void OnDrag()
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
        Item emptyItem = new Item(-1, new int[1, 1] { { 0 } });
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
