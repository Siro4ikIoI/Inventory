using System;
using System.Linq;

public class Matrix
{
    private readonly int[,] _matrix;

    public int this[int row_key, int col_key]
    {
        get => _matrix[row_key, col_key];
        private set => _matrix[row_key, col_key] = value;
    }

    public (int row, int col) Size
    {
        get => new (_matrix.GetLength(0), _matrix.GetLength(1));
    }

    public Matrix((int row, int col) shape)
    {
        _matrix = new int[shape.row, shape.col];
    }

    public Matrix(int [,] values)
    {
        _matrix = values.Clone() as int[,];
    }

    public Matrix Add(Matrix other)
    {
        if (Size != other.Size)
            throw new Exception("Different shapes");

        Matrix result = new Matrix(Size);
        for (int i = 0; i < Size.row; i++)
        {
            for (int j = 0; j < Size.col; j++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        }
        return result;
    }

    public Matrix Substract(Matrix other)
    {
        Matrix negativeOther = new Matrix(other._matrix);
        for (int i = 0; i < Size.row; i++)
        {
            for (int j = 0; j < Size.col; j++)
            {
                negativeOther[i, j] *= -1;
            }
        }

        return this.Add(negativeOther);
    }

    public bool Reshape((int row, int col) newShape, out Matrix result, int OffestRow = 0, int OffsetCol = 0)
    {
        result = new Matrix(newShape);
        for (int i = 0; i < Size.row; i++)
        {
            int newi = i + OffestRow;
            if (newi < 0 || newi >= result.Size.row)
                return false;
            
            for (int j = 0; j < Size.col; j++)
            {
                int newj = j + OffsetCol;
                if (newj < 0 || newj >= result.Size.col)
                    return false;

                result[newi, newj] = this[i, j];
            }
        }
        return true;
    }

    public bool GetSubmatrix((int row, int col) subShape, int offsetRow, int offsetCol, out Matrix result)
    {
        result = new Matrix(subShape);

        for (int i = 0; i < subShape.row; i++)
        {
            for (int j = 0; j < subShape.col; j++)
            {
                int sourceRow = i + offsetRow;
                int sourceCol = j + offsetCol;

                if (sourceRow < 0 || sourceRow >= Size.row ||
                    sourceCol < 0 || sourceCol >= Size.col)
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
        int[,] transposed = new int[Size.col, Size.row];
        for (int i = 0; i < Size.row; i++)
        {
            for (int j = 0; j < Size.col; j++)
            {
                transposed[j, i] = this[Size.row - i - 1, j];
            }
        }

        return new Matrix(transposed);
    }
}
