using System;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    public event Action<Item, (int row, int col)> ItemAdded;
    public event Action<Item> ItemExtracted;
    public event Action<int[,]> CollisionWhenAdding;

    private Dictionary<int, (int row, int col)> _positions = new();
    private Dictionary<int, Item> _items = new();
    private Dictionary<int, Item> _hiddenItems = new();

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
        return _items.ContainsKey(item.Id);
    }

    public bool IsEmpty() => _items.Count == 0;

    public bool TryAddItem(Item item, (int row, int col) position)
    {
        if (!CanAddItem(item, position))
            return false;

        item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.row, position.col);
        _cells = _cells.Add(itemMatrix);

        bool isHidden = _hiddenItems.ContainsKey(item.Id);
        if (isHidden)
        {
            _items[item.Id] = item;
            _positions[item.Id] = position;
            _hiddenItems.Remove(item.Id);
        }
        else
        {
            _items.Add(item.Id, item);
            _positions.Add(item.Id, position);
        }

        ItemAdded?.Invoke(item, position);
        return true;
    }

    public bool TryExtractItem(Item item, out (int row, int col) position)
    {
        position = new (-1, -1);

        if (!ContainsItem(item))
            return false;

        position = _positions[item.Id];

        bool isHidden = _hiddenItems.ContainsKey(item.Id);

        if (!isHidden)
        {
            item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.row, position.col);
            _cells = _cells.Substract(itemMatrix);
        }
        else
        {
            _hiddenItems.Remove(item.Id);
        }

        _items.Remove(item.Id);
        _positions.Remove(item.Id);

        ItemExtracted?.Invoke(item);
        return true;
    }

    public bool CanAddItem(Item item, (int row, int col) position)
    {
        if (ContainsItem(item) && !_hiddenItems.ContainsKey(item.Id))
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
        
        return collisionMatrix.Max() <= (int)CellType.FILL;
    }

    public Matrix ToMatrix()
    {
        return _cells;
    }

    public void HideItem(Item item)
    {
        if (!ContainsItem(item))
            return;

        if (_hiddenItems.ContainsKey(item.Id))
            return;

        Pair position = _positions[item.Id];
        item.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.row, position.col);
        _cells = _cells.Substract(itemMatrix);

        Item itemCopy = (Item)item.Clone();
        _items[item.Id] = itemCopy;

        _hiddenItems.Add(item.Id, itemCopy);
        ItemExtracted?.Invoke(item);
    }

    public void RestoreItem(Item item)
    {
        if (!ContainsItem(item))
            return;

        if (!_hiddenItems.ContainsKey(item.Id))
            return;

        Item hiddenItem = _hiddenItems[item.Id];
        Pair position = _positions[item.Id];

        hiddenItem.ToMatrix().Reshape(_cells.Size, out Matrix itemMatrix, position.row, position.col);
        _cells = _cells.Add(itemMatrix);

        _hiddenItems.Remove(hiddenItem.Id);
        ItemAdded?.Invoke(hiddenItem, position);
    }
}
