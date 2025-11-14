public class DragAndDropCreator
{
    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public DragAndDropCreator(ModelCollection modelCollection, CanvasView canvasView)
    {
        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnItemAdded(Item item)
    {
        ItemView itemView = _canvasView.GetItemView(item.Id);

        DragAndDropModel dragAndDropModel = new DragAndDropModel(item);
        DragAndDropPresenter dragAndDropPresenter = new DragAndDropPresenter(dragAndDropModel, itemView.DragAndDrop, 
                                                                                _modelCollection, _canvasView);

        dragAndDropPresenter.Enable();
    }

    public void Enable()
    {
        _modelCollection.ItemAdded += OnItemAdded;
    }

    public void Disable()
    {
        _modelCollection.ItemAdded -= OnItemAdded;
    }
}
