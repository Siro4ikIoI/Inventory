using System;
using System.Linq;

public class Matrix
{
    private readonly int[,] _matrix;

    public int this[int r_key, int c_key]
    {
        get => _matrix[r_key, c_key];
        private set => _matrix[r_key, c_key] = value;
    }

    public Pair Size
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
        if (this.Size != other.Size)
            throw new Exception("Different shapes");

        Matrix result = new Matrix(this.Size);
        for (int i = 0; i < this.Size.Row; i++)
        {
            for (int j = 0; j < this.Size.Col; j++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        }
        return result;
    }

    public Matrix Substract(Matrix other)
    {
        Matrix negativeOther = new Matrix(other._matrix);
        for (int i = 0; i < this.Size.Row; i++)
        {
            for (int j = 0; j < this.Size.Col; j++)
            {
                negativeOther[i, j] *= -1;
            }
        }

        return this.Add(negativeOther);
    }

    public bool Reshape(Pair newShape, out Matrix result, int OffestRow = 0, int OffsetCol = 0)
    {
        result = new Matrix(newShape);
        for (int i = 0; i < this.Size.Row; i++)
        {
            int newi = i + OffestRow;
            if (newi < 0 || newi >= result.Size.Row)
                return false;
            
            for (int j = 0; j < this.Size.Col; j++)
            {
                int newj = j + OffsetCol;
                if (newj < 0 || newj >= result.Size.Col)
                    return false;

                result[newi, newj] = this[i, j];
            }
        }
        return true;
    }

    public bool GetSubmatrix(Pair subShape, int offsetRow, int offsetCol, out Matrix result)
    {
        result = new Matrix(subShape);

        for (int i = 0; i < subShape.Row; i++)
        {
            for (int j = 0; j < subShape.Col; j++)
            {
                int sourceRow = i + offsetRow;
                int sourceCol = j + offsetCol;

                if (sourceRow < 0 || sourceRow >= this.Size.Row ||
                    sourceCol < 0 || sourceCol >= this.Size.Col)
                {
                    return false;
                }

                result[i, j] = this[sourceRow, sourceCol];
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

    public int[,] GetStructure()
    {
        return (int[,]) _matrix.Clone();
    }

    public Matrix Rotate()
    {
        int[,] transposed = new int[Size.Col, Size.Row];
        for (int i = 0; i < Size.Row; i++)
        {
            for (int j = 0; j < Size.Col; j++)
            {
                transposed[j, i] = this[Size.Row - i - 1, j];
            }
        }

        return new Matrix(transposed);
    }
}
