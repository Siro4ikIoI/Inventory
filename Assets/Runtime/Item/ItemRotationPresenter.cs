public class ItemRotationPresenter
{
    private InputHandler _input;

    private ModelCollection _modelCollection;

    public ItemRotationPresenter(InputHandler input, ModelCollection modelCollection)
    {
        _input = input;

        _modelCollection = modelCollection;
    }

    private void OnRightClicked()
    {
        if (_modelCollection.GetCurrentDragAndDrop() == null)
            return;

        ItemModel item = _modelCollection.GetCurrentDragAndDrop().Item;
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
