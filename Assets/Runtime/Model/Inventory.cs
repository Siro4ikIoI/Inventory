using System.Collections.Generic;
using System.Linq;

public class Inventory : IMatrix
{
    private Dictionary<Item, Pair> _items = new();

    private Matrix _cells;
    public Pair Shape { get { return _cells.Shape; } }

    public Inventory(Pair shape)
    {
        _cells = new Matrix(shape);
    }

    public bool ContainsItem(Item item)
    {
        return _items.Keys.Contains(item);
    }

    public bool IsEmpty()
    {
        if (_items.Count == 0 )
        {
            return true;
        }
        return false;
    }

    public bool TryAddItem(Item item, Pair pos)
    {
        if (ContainsItem(item))
            return false;

        bool isItemInBorder = item.ToMatrix().Reshape(_cells.Shape, out Matrix itemMatrix, pos.Row, pos.Col);
        if (!isItemInBorder)
            return false;

        Matrix newCells = _cells.Add(itemMatrix);
        if (newCells.Max() > (int)CellType.FILL)
            return false;

        _items.Add(item, pos);
        _cells = newCells;
        return true;
    }

    public bool TryExtractItem(Item item, out Pair pos)
    {
        pos = new Pair(-1, -1);

        if (!ContainsItem(item))
            return false;

        pos = _items[item];
        item.ToMatrix().Reshape(_cells.Shape, out Matrix itemMatrix, pos.Row, pos.Col);
        _cells = _cells.Substract(itemMatrix);
        _items.Remove(item);
        return true;
    }

    public Matrix ToMatrix()
    {
        return _cells;
    }
}
