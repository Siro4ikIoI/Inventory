using System;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private RectTransform inventoryArea;
    [SerializeField] private float cellSize;
    [SerializeField] private float cellBorder;

    private RectTransform rectTransform;
    public int Row { get; private set; }
    public int Col { get; private set; }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (inventoryArea == null)
        {
            inventoryArea = rectTransform;
        }
    }

    public void SetShape(int row, int col)
    {
        Row = row;
        Col = col;
    }

    // Проверяет, находится ли точка внутри области инвентаря
    public bool IsPointInside(Vector2 screenPosition, Camera camera = null)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            inventoryArea,
            screenPosition,
            camera
        );
    }

    public Vector2 GetLocalPosition(Vector2 tablePosition)
    {
        float x = tablePosition.x * cellSize + ((2 * tablePosition.x + 1) * cellBorder);
        float y = tablePosition.y * cellSize + ((2 * tablePosition.y + 1) * cellBorder);

        return new Vector2(x, y);
    }

    // Получить локальную позицию внутри инвентаря
    public bool GetTablePosition(Vector2 screenPosition, out Vector2 localPosition, Camera camera = null)
    {
        bool b = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryArea,
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

        return b;
    }

    public RectTransform GetContainer()
    {
        return inventoryArea;
    }
}
