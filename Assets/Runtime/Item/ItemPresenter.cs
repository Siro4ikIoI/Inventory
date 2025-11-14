public class ItemPresenter
{
    private Item _item;
    private ItemView _itemView;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public ItemPresenter(Item item, ItemView itemView, ModelCollection modelCollection, CanvasView canvasView)
    {
        _item = item;
        _itemView = itemView;

        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnItemRotated(Direction direction)
    {
        _itemView.SetRotation(direction, _item.ToMatrix().GetStructure());
    }

    public void Enable()
    {
        _item.Rotated += OnItemRotated;
    }

    public void Disable()
    {
        _item.Rotated -= OnItemRotated;
    }
}
