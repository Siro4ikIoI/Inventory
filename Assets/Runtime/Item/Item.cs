using System;

public class Item
{
    public event Action<Direction> Rotated;

    private Matrix _blocks;

    public int Id { get; private set; }

    private Direction _rotation;

    public Item(int id, int[,] blocks)
    {
        Id = id;
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
}
