using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public event Action<Item, (int row, int col)> ItemAdded;
    public event Action<Item> ItemExtracted;
    public event Action<int[,]> CollisionWhenAdding;

    private Dictionary<Item, (int row, int col)> _items = new();

    private Matrix _cells;
    public InventoryType Type { get; private set; }
    public (int row, int col) Shape { get { return _cells.Size; } }

    public Inventory(InventoryType type, (int row, int col) shape)
    {
        Type = type;
        _cells = new Matrix(shape);
    }

    public bool ContainsItem(Item item)
    {
        return _items.Keys.Contains(item);
    }

    public bool IsEmpty() => _items.Count == 0;

    public bool TryAddItem(Item item, (int row, int col) position)
    {
        if (ContainsItem(item))
            return false;

        bool isItemInBorder = item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.row, position.col);
        if (!isItemInBorder)
            return false;

        Matrix newCells = _cells.Add(itemMatrix);
        if (newCells.Max() > (int)CellType.FILL)
            return false;

        _items.Add(item, position);
        _cells = newCells;

        ItemAdded?.Invoke(item, position);
        return true;
    }

    public bool TryExtractItem(Item item, out (int row, int col) position)
    {
        position = new (-1, -1);

        if (!ContainsItem(item))
            return false;

        position = _items[item];
        item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.row, position.col);
        _cells = _cells.Substract(itemMatrix);
        _items.Remove(item);

        ItemExtracted?.Invoke(item);
        return true;
    }

    public bool CanAddItem(Item item, (int row, int col) position)
    {
        if (_items.ContainsKey(item))
            return false;

        Matrix itemMatrix = item.ToMatrix();
        bool isInBounds = _cells.GetSubmatrix(itemMatrix.Size, position.row, position.col, out Matrix submatrix);
        if (!isInBounds)
        {
            int[,] emptyArray = new int[Shape.row, Shape.col];
            CollisionWhenAdding?.Invoke(emptyArray);
            return false;
        }

        Matrix sumMatrix = itemMatrix.Add(submatrix);
        int[,] sumArray = new int[sumMatrix.Size.row, sumMatrix.Size.col];
        for (int i = 0; i < sumMatrix.Size.row; i++)
        {
            for (int j = 0; j < sumMatrix.Size.col; j++)
            {
                if (sumMatrix[i, j] > 1)
                    sumArray[i, j] = sumMatrix[i, j];
                else
                    sumArray[i, j] = itemMatrix[i, j];
            }
        }

        new Matrix(sumArray).Reshape(Shape, out Matrix collisionMatrix, position.row, position.col);
        CollisionWhenAdding?.Invoke(collisionMatrix.GetStructure());
        return collisionMatrix.Max() > (int)CellType.FILL;
    }

    public Matrix ToMatrix()
    {
        return _cells;
    }
}
