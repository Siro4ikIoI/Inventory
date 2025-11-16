using System;

public class DragAndDropContainer
{
    public event Action<DragAndDropModel> ChangedDragAndDrop;

    private DragAndDropModel _currentDragAndDrop = null;

    public void SetCurrentDragAndDrop(DragAndDropModel dragAndDropModel)
    {
        _currentDragAndDrop = dragAndDropModel;
        ChangedDragAndDrop?.Invoke(_currentDragAndDrop);
    }

    public DragAndDropModel GetCurrentDragAndDrop()
    {
        return _currentDragAndDrop;
    }
}
