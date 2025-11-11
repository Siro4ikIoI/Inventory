using System;
using System.Collections.Generic;

public class GameState
{
    private const int POINTS_PER_CELL = 10;

    private int _score;
    private HashSet<int> _scoredItems = new HashSet<int>();

    public int Score
    {
        get => _score;
        private set => _score = value;
    }

    public event Action<int> ScoreChanged;

    public void AddScore(Item item, int itemId)
    {
        if (_scoredItems.Contains(itemId))
            return; // Очки за этот предмет уже начислены

        int points = CalculatePoints(item);
        _score += points;
        _scoredItems.Add(itemId);
        ScoreChanged?.Invoke(_score);
    }

    private int CalculatePoints(Item item)
    {
        int filledCells = CountFilledCells(item);
        return filledCells * POINTS_PER_CELL;
    }

    private int CountFilledCells(Item item)
    {
        Matrix matrix = item.ToMatrix();
        int count = 0;

        for (int i = 0; i < matrix.Shape.Row; i++)
        {
            for (int j = 0; j < matrix.Shape.Col; j++)
            {
                if (matrix[i, j] == (int)CellType.FILL)
                    count++;
            }
        }

        return count;
    }

    public void ResetScore()
    {
        _score = 0;
        _scoredItems.Clear();
        ScoreChanged?.Invoke(_score);
    }
}