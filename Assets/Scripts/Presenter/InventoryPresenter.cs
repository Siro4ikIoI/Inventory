using System.Collections.Generic;
using System.Linq;

public class InventoryPresenter
{
    private Dictionary<InventoryType, Inventory> _inventories = new();
    private Dictionary<int, Item> _items = new();
    private CanvasView _canvas;

    private Inventory _draggedItemInventory = null;
    private Pair _draggedItemPosition;

    public InventoryPresenter(CanvasView canvas)
    {
        _inventories.Add(InventoryType.CASE, new Inventory(new Pair(7, 4)));
        _inventories.Add(InventoryType.INVENTARY, new Inventory(new Pair(5, 9)));
        
        _canvas = canvas;
        foreach(var inventoryPair in _inventories)
        {
            _canvas.InitializeInventory(inventoryPair.Key, inventoryPair.Value.Shape.Row, inventoryPair.Value.Shape.Col);
        }

        _canvas.ItemDroped += OnItemDroped;
        _canvas.ItemDragPositionChanged += OnItemDragPositionChanged;
        _canvas.ItemBeginDrag += OnItemBeginDrag;
    }

    private void OnItemBeginDrag(int itemId)
    {
        if (!_items.ContainsKey(itemId))
            return;

        Item item = _items[itemId];
        Inventory sourceInventory = _inventories.First(i => i.Value.ContainsItem(item)).Value;

        if (sourceInventory.TryExtractItem(item, out Pair position))
        {
            _draggedItemInventory = sourceInventory;
            _draggedItemPosition = position;
        }
    }

    private bool OnItemDroped(int itemId, InventoryType inventoryType, int row, int col)
    {
        Item item = _items[itemId];
        Inventory newInventory = _inventories[inventoryType];
        if (!newInventory.TryAddItem(item, new Pair(row, col)))
        {
            _draggedItemInventory.TryAddItem(item, _draggedItemPosition);
            _draggedItemInventory = null;
            return false;
        }
        _draggedItemInventory = null;
        return true;
    }

    private void OnItemDragPositionChanged(int itemId, InventoryType inventoryType, int row, int col)
    {
        if (!_items.ContainsKey(itemId))
            return;

        Item item = _items[itemId];
        Inventory targetInventory = _inventories[inventoryType];

        Matrix itemMatrix = item.ToMatrix();
        Pair inventoryShape = targetInventory.Shape;

        // Выделяем подматрицу из инвентаря
        Matrix submatrix = targetInventory.ToMatrix().GetSubmatrix(itemMatrix.Shape, row, col);

        // Складываем матрицу предмета с подматрицей
        Matrix sumMatrix = itemMatrix.Add(submatrix);

        // Создаём массив для подсветки
        int[,] highlightArray = new int[inventoryShape.Row, inventoryShape.Col];

        // Проверяем, входит ли предмет в границы
        bool isInBounds = itemMatrix.Reshape(inventoryShape, out Matrix reshapedItemMatrix, row, col);

        // Заполняем массив подсветки
        for (int i = 0; i < inventoryShape.Row; i++)
        {
            for (int j = 0; j < inventoryShape.Col; j++)
            {
                if (reshapedItemMatrix[i, j] == 0)
                {
                    // Ячейка не затронута предметом
                    highlightArray[i, j] = 0;
                }
                else
                {
                    // Вычисляем локальную позицию в матрице суммы
                    int localRow = i - row;
                    int localCol = j - col;

                    if (localRow >= 0 && localRow < sumMatrix.Shape.Row &&
                        localCol >= 0 && localCol < sumMatrix.Shape.Col)
                    {
                        int sumValue = sumMatrix[localRow, localCol];

                        if (sumValue == 1)
                        {
                            // Можно разместить (зелёный)
                            highlightArray[i, j] = 1;
                        }
                        else if (sumValue >= 2)
                        {
                            // Конфликт (красный)
                            highlightArray[i, j] = 2;
                        }
                    }
                    else
                    {
                        // Выход за границы (красный)
                        highlightArray[i, j] = 2;
                    }
                }
            }
        }

        // Если предмет выходит за границы, все его ячейки красные
        if (!isInBounds)
        {
            for (int i = 0; i < inventoryShape.Row; i++)
            {
                for (int j = 0; j < inventoryShape.Col; j++)
                {
                    if (reshapedItemMatrix[i, j] != 0)
                    {
                        highlightArray[i, j] = 2;
                    }
                }
            }
        }

        // Конвертируем в Matrix и передаём во View
        Matrix highlightMatrix = new Matrix(highlightArray);
        _canvas.HighlightInventoryCells(inventoryType, highlightMatrix);
    }

    public void Initialize(ItemSO itemSO)
    {
        ItemSettings itemSettings = itemSO.items[0];
        Pair[] positions =
        {
            new Pair(0, 0), new Pair(0, 2), new Pair(2, 0)
        };

        GenerationPlug(itemSettings, positions);
    }
    
    private void GenerationPlug(ItemSettings itemSettings, Pair[] positions)
    {
        int id = 1;
        foreach (var pos in positions)
        {
            Item item = new Item(id, itemSettings.GetStructure());
            _items.Add(id, item);
            _inventories[InventoryType.CASE].TryAddItem(item, pos);
            _canvas.SpawnItem(id, itemSettings.item, pos.Row, pos.Col);
            id++;
        }
    }
}
