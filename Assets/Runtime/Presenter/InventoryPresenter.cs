using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPresenter
{
    private Dictionary<InventoryType, Inventory> _inventories = new();
    private Dictionary<int, Item> _items = new();
    private CanvasView _canvas;
    private ItemSO _itemSO;

    private Inventory _previousItemInventory = null;
    private Pair _previousItemPosition;
    private Item _currentItem;
    private Direction _lastItemRotation;

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
        _canvas.ItemRotation += OnItemRotation;
    }

    private void OnItemRotation()
    {
        if (_currentItem == null)
            return;

        _currentItem.Rotate();
        _canvas.RotateItem(_currentItem.Id, _currentItem.GetRotation(), _currentItem.ToMatrix().GetStructure());
    }

    private void OnItemBeginDrag(int itemId)
    {
        if (!_items.ContainsKey(itemId))
            return;

        Item item = _items[itemId];
        Inventory sourceInventory = _inventories.First(i => i.Value.ContainsItem(item)).Value;

        if (sourceInventory.TryExtractItem(item, out Pair position))
        {
            _previousItemInventory = sourceInventory;
            _previousItemPosition = position;
        }

        _currentItem = item;
        _lastItemRotation = item.GetRotation();
    }

    private bool OnItemDroped(int itemId, InventoryType inventoryType, int row, int col)
    {
        Item item = _items[itemId];
        if (inventoryType == InventoryType.NONE || !_inventories[inventoryType].TryAddItem(item, new Pair(row, col)))
        {
            _currentItem.SetRotation(_lastItemRotation);
            _canvas.RotateItem(_currentItem.Id, _currentItem.GetRotation(), _currentItem.ToMatrix().GetStructure());
            _previousItemInventory.TryAddItem(item, _previousItemPosition);
            
            _previousItemInventory = null;
            _currentItem = null;
            return false;
        }

        if (_previousItemInventory.IsEmpty() && inventoryType == InventoryType.INVENTARY)
        {
            GenerationObjects(3);
        }

        _previousItemInventory = null;
        _currentItem = null;
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
        int[,] sumArray = new int[sumMatrix.Shape.Row, sumMatrix.Shape.Col];
        for (int i = 0; i < sumMatrix.Shape.Row; i++)
        {
            for (int j = 0; j < sumMatrix.Shape.Col; j++)
            {
                if (sumMatrix[i, j] > 1)
                    sumArray[i, j] = sumMatrix[i, j];
                else
                    sumArray[i, j] = itemMatrix[i, j];
            }
        }

        new Matrix(sumArray).Reshape(inventoryShape, out Matrix highlightMatrix, row, col);

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
        _itemSO = itemSO;
        GenerationObjects(3);
    }

    private void GenerationObjects(int count)
    {
        Inventory inventory = _inventories[InventoryType.INVENTARY];

        Matrix workingMatrix = (Matrix)inventory.ToMatrix().Clone();

        for (int i = 0; i < count; i++)
        {
            bool placed = false;
            Pair pos = new Pair(i * 2, 0);

            List<ItemSettings> items = _itemSO.items.OrderBy(_ => Random.value).ToList();

            int attempt = 0;
            while (attempt < items.Count && !placed)
            {
                var itemSettings = items[attempt];
                int id = _items.Count > 0 ? _items.Keys.Max() + 1 : 0;
                var item = new Item(id, itemSettings.GetStructure());

                if (TryFindFreePosition(item, workingMatrix, out var actualPos))
                {
                    _items.Add(item.Id, item);
                    _inventories[InventoryType.CASE].TryAddItem(item, pos);
                    _canvas.SpawnItem(item.Id, itemSettings.item, pos.Row, pos.Col);

                    item.ToMatrix().Reshape(workingMatrix.Shape, out var itemMatrix, actualPos.Row, actualPos.Col);
                    workingMatrix = workingMatrix.Add(itemMatrix);

                    placed = true;
                }
                else
                {
                    items.RemoveAt(attempt);
                    continue;
                }

                attempt++;
            }
        }
    }

    private bool TryFindFreePosition(Item item, Matrix inventory, out Pair foundPos)
    {
        foundPos = new Pair(0, 0);

        for (int row = 0; row < inventory.Shape.Row; row++)
        {
            for (int col = 0; col < inventory.Shape.Col; col++)
            {
                bool inBounds = item.ToMatrix().Reshape(inventory.Shape, out Matrix itemMatrix, row, col);
                if (!inBounds)
                    continue;

                Matrix newCells = inventory.Add(itemMatrix);
                if (newCells.Max() <= (int)CellType.FILL)
                {
                    foundPos = new Pair(row, col);
                    return true;
                }
            }
        }
        return false;
    }
}
