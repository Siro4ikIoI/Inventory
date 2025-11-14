using System;

public class DragAndDropModel
{
    public event Action Draged;
    public event Action EndDraged;

    private Item _item;
    public Item Item { get { return _item; } }
    
    public Inventory PreviousItemInventory { get; set; }
    public Pair PreviousItemPosition { get; set; }

    public Direction PreviousItemRotation { get; set; }

    public InventoryType CurrentInventory { get; set; }

    public Pair CurrentPosition { get; set; }

    public DragAndDropModel(Item item)
    {
        _item = item;
    }

    public void Drag()
    {
        Draged?.Invoke();
    }

    public void EndDrag()
    {
        EndDraged?.Invoke();
    }
}
