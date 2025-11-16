using System;

public class DragAndDropModel
{
    public event Action<float, float> Draged;
    public event Action EndDraged;
    public event Action InventorySetted;
    public event Action Destroyed;

    private Item _item;
    public Item Item { get { return _item; } }

    public Inventory PreviousItemInventory { get; set; }

    public InventoryType CurrentInventory { get; private set; }

    public (int row, int col) CurrentPosition { get; private set; }

    public DragAndDropModel(Item item)
    {
        _item = item;
    }

    public void Drag(float x, float y)
    {
        Draged?.Invoke(x, y);
    }

    public void SetInventoryAndPosition(InventoryType inventory, (int row, int col) position)
    {
        CurrentInventory = inventory;
        CurrentPosition = position;
        InventorySetted?.Invoke();
    }

    public void EndDrag()
    {
        EndDraged?.Invoke();
    }

    public void Destroy()
    {
        Destroyed?.Invoke();
    }
}
