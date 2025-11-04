using System;
using System.Linq;

public class Matrix : ICloneable
{
    private readonly int[,] _matrix;

    public int this[int r_key, int c_key]
    {
        get => _matrix[r_key, c_key];
        private set => _matrix[r_key, c_key] = value;
    }

    public Pair Shape
    {
        get => new Pair(_matrix.GetLength(0), _matrix.GetLength(1));
    }

    public Matrix(Pair shape)
    {
        _matrix = new int[shape.Row, shape.Col];
    }

    public Matrix(int [,] values)
    {
        _matrix = values.Clone() as int[,];
    }

    public Matrix Add(Matrix other)
    {
        if (this.Shape != other.Shape)
            throw new Exception("Different shapes");

        Matrix result = new Matrix(this.Shape);
        for (int i = 0; i < this.Shape.Row; i++)
        {
            for (int j = 0; j < this.Shape.Col; j++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        }
        return result;
    }

    public bool Reshape(Pair newShape, out Matrix result, int OffestRow = 0, int OffsetCol = 0)
    {
        result = new Matrix(newShape);
        for (int i = 0; i < this.Shape.Row; i++)
        {
            int newi = i + OffestRow;
            if (newi < 0 || newi >= result.Shape.Row)
                return false;
            
            for (int j = 0; j < this.Shape.Col; j++)
            {
                int newj = j + OffsetCol;
                if (newj < 0 || newj >= result.Shape.Col)
                    return false;

                result[newi, newj] = this[i, j];
            }
        }
        return true;
    }

    public int Max()
    {
        return _matrix.Cast<int>().Max();
    }

    public int Min()
    {
        return _matrix.Cast<int>().Min();
    }

    public object Clone()
    {
        return new Matrix(_matrix);
    }
}
