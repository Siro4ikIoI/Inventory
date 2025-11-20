using System;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private RectTransform _inventoryArea;
    [SerializeField] private float _cellSize;
    [SerializeField] private float _cellBorder;

    public int Row { get; private set; }
    public int Col { get; private set; }
    private CellView[,] _cells;

    public void SetShape(int row, int col)
    {
        Row = row;
        Col = col;
        InitializeCells();
    }

    private void InitializeCells()
    {
        _cells = new CellView[Row, Col];

        CellView[] existingCells = _inventoryArea.GetComponentsInChildren<CellView>();

        int index = 0;
        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                if (index < existingCells.Length)
                {
                    _cells[i, j] = existingCells[index];
                    index++;
                }
            }
        }
    }

    // Проверяет, находится ли точка внутри области инвентаря
    public bool IsPointInside(Vector2 screenPosition, Camera camera = null)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            _inventoryArea,
            screenPosition,
            camera
        );
    }

    public Vector2 GetLocalPosition(Vector2 tablePosition)
    {
        float x = tablePosition.x * _cellSize + ((2 * tablePosition.x + 1) * _cellBorder);
        float y = tablePosition.y * _cellSize + ((2 * tablePosition.y + 1) * _cellBorder);

        return new Vector2(x, y);
    }

    // Получить локальную позицию внутри инвентаря
    public bool GetTablePosition(Vector2 screenPosition, out Vector2 localPosition, Camera camera = null)
    {
        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _inventoryArea,
            screenPosition,
            camera,
            out localPosition
        );

        Rect rect = GetContainer().rect;
        float normalizedX = (localPosition.x - rect.xMin) / rect.width;
        float normalizedY = (localPosition.y - rect.yMin) / rect.height;

        int rowIndex = (int)Math.Round(normalizedY * Row, MidpointRounding.AwayFromZero);
        int colIndex = (int)Math.Round(normalizedX * Col, MidpointRounding.AwayFromZero);

        localPosition = new Vector2(colIndex, rowIndex);

        return success;
    }

    public void HighlightCells(int[,] highlightArray)
    {
        if (_cells == null) return;

        for (int i = 0; i < Row; i++)
        {
            for (int j = 0; j < Col; j++)
            {
                if (_cells[i, j] != null && highlightArray != null)
                {
                    int state = highlightArray[i, j];
                    _cells[i, j].SetHighlight(state);
                }
            }
        }
    }

    public RectTransform GetContainer()
    {
        return _inventoryArea;
    }
}
