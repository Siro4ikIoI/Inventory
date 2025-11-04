using System.Collections.Generic;
using System.Linq;

public class Inventory : IMatrix
{
    private Dictionary<Pair, Item> _items = new();

    private Matrix _cells;

    public Inventory(Pair shape)
    {
        _cells = new Matrix(shape);
    }

    public bool TryAddItem(Item item, Pair pos)
    {
        if (_items.Values.Contains(item))
            return false;

        bool isItemInBorder = item.ToMatrix().Reshape(_cells.Shape, out Matrix itemMatrix, pos.Row, pos.Col);
        if (!isItemInBorder)
            return false;

        Matrix newCells = _cells.Add(itemMatrix);
        if (newCells.Max() > (int)CellType.FILL)
            return false;

        _items.Add(pos, item);
        _cells = newCells;
        return true;
    }

    public Matrix ToMatrix()
    {
        return _cells;
    }
}
