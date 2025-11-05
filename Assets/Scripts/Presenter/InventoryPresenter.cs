using System.Collections.Generic;
using System.Linq;

public class InventoryPresenter
{
    private Dictionary<InventoryType, Inventory> _inventories = new();
    private Dictionary<int, Item> _items = new();
    private CanvasView _canvas;

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

        // TODO проверка на пустоту кейса и генераци€ в случае если он пуст

        return true;
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

    // TODO «аменить заглушку на метод генерации
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

    // TODO ћетод дл€ определени€ набора генерируемых предметов

    // TODO ћетод обработки перемещени€ предмета
}
