using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryPresenter
{
    private Dictionary<InventoryType, Inventory> _inventories = new();
    private Dictionary<int, Item> _items = new();
    private CanvasView _canvas;
    private ItemSO _itemSO;

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
    }

    private bool OnItemDroped(int itemId, InventoryType inventoryType, int row, int col)
    {
        Item item = _items[itemId];
        Inventory oldInventory = _inventories.First(i => i.Value.ContainsItem(item)).Value;
        oldInventory.TryExtractItem(item, out Pair oldPosition);

        Inventory newInventory = _inventories[inventoryType];
        if (!newInventory.TryAddItem(item, new Pair(row, col)))
        {
            oldInventory.TryAddItem(item, oldPosition);
            return false;
        }

        if (oldInventory.IsEmpty() && inventoryType == InventoryType.INVENTARY)
        {
            GenerationObjects(3);
        }

        return true;
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
