using System;

public class ItemModel : ICloneable
{
    public event Action<Direction> Rotated;

    private Matrix _blocks;

    public int Id { get; private set; }
    public ItemType Type { get; private set; }

    private Direction _rotation;

    public ItemModel(int id, ItemType type, int[,] blocks)
    {
        Id = id;
        Type = type;
        _blocks = new(blocks);
        _rotation = Direction.N;
    }

    public Matrix ToMatrix()
    {
        return _blocks;
    }

    public void Rotate()
    {
        _blocks = _blocks.Rotate();
        _rotation++;

        Rotated?.Invoke(_rotation);
    }

    public void SetRotation(Direction direction)
    {
        int rotationAmount = direction - _rotation;
        if (rotationAmount < 0)
        {
            rotationAmount += 4;
        }

        for (int i = 0; i < rotationAmount; i++)
        {
            Rotate();
        }

        Rotated?.Invoke(_rotation);
    }

    public Direction GetRotation()
    {
        return _rotation;
    }

    public object Clone()
    {
        ItemModel newItem = new ItemModel(Id, Type, _blocks.GetStructure());
        newItem._rotation = _rotation;
        return newItem;
    }
}
