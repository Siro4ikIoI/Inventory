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

        bool isInBounds = targetInventory.ToMatrix().GetSubmatrix(itemMatrix.Shape, row, col, out Matrix submatrix);

        if (!isInBounds)
        {
            int[,] emptyArray = new int[inventoryShape.Row, inventoryShape.Col];
            _canvas.HighlightInventoryCells(inventoryType, emptyArray);
            return;
        }

        Matrix sumMatrix = itemMatrix.Add(submatrix);

        sumMatrix.Reshape(inventoryShape, out Matrix highlightMatrix, row, col);

        // Конвертируем в массив и передаём во View
        int[,] highlightArray = new int[inventoryShape.Row, inventoryShape.Col];
        for (int i = 0; i < inventoryShape.Row; i++)
        {
            for (int j = 0; j < inventoryShape.Col; j++)
            {
                int value = highlightMatrix[i, j];                
                highlightArray[i, j] = value;
            }
        }

        _canvas.HighlightInventoryCells(inventoryType, highlightArray);
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
