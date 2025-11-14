public class ItemRotationPresenter
{
    private InputHandler _input;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public ItemRotationPresenter(InputHandler input, ModelCollection modelCollection, CanvasView canvasView)
    {
        _input = input;

        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void OnRightClicked()
    {
        if (_modelCollection.GetCurrentDragAndDrop() == null)
            return;

        Item item = _modelCollection.GetCurrentDragAndDrop().Item;
        item.Rotate();
    }

    public void Enable()
    {
        _input.RightClick += OnRightClicked;
    }

    public void Disable()
    {
        _input.RightClick -= OnRightClicked;
    }
}
