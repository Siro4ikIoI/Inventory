using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public event Action<Item, Pair> ItemAdded;
    public event Action<Item> ItemExtracted;
    public event Action<int[,]> CollisionWhenAdding;

    private Dictionary<Item, Pair> _items = new();

    private Matrix _cells;
    public Pair Shape { get { return _cells.Size; } }

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

    public bool TryAddItem(Item item, Pair position)
    {
        if (ContainsItem(item))
            return false;

        bool isItemInBorder = item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.Row, position.Col);
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

    public bool TryExtractItem(Item item, out Pair position)
    {
        position = new Pair(-1, -1);

        if (!ContainsItem(item))
            return false;

        position = _items[item];
        item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.Row, position.Col);
        _cells = _cells.Substract(itemMatrix);
        _items.Remove(item);

        ItemExtracted?.Invoke(item);
        return true;
    }

    public bool CanAddItem(Item item, Pair position)
    {
        if (_items.ContainsKey(item))
            return false;

        Matrix itemMatrix = item.ToMatrix();
        bool isInBounds = _cells.GetSubmatrix(itemMatrix.Size, position.Row, position.Col, out Matrix submatrix);
        if (!isInBounds)
        {
            int[,] emptyArray = new int[Shape.Row, Shape.Col];
            CollisionWhenAdding?.Invoke(emptyArray);
            return false;
        }

        Matrix sumMatrix = itemMatrix.Add(submatrix);
        int[,] sumArray = new int[sumMatrix.Size.Row, sumMatrix.Size.Col];
        for (int i = 0; i < sumMatrix.Size.Row; i++)
        {
            for (int j = 0; j < sumMatrix.Size.Col; j++)
            {
                if (sumMatrix[i, j] > 1)
                    sumArray[i, j] = sumMatrix[i, j];
                else
                    sumArray[i, j] = itemMatrix[i, j];
            }
        }

        new Matrix(sumArray).Reshape(Shape, out Matrix collisionMatrix, position.Row, position.Col);
        CollisionWhenAdding?.Invoke(collisionMatrix.GetStructure());
        return collisionMatrix.Max() > (int)CellType.FILL;
    }

    public Matrix ToMatrix()
    {
        return _cells;
    }
}
