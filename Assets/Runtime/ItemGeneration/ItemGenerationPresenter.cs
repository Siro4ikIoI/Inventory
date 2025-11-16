using System.Collections.Generic;

public class ItemGenerationPresenter
{
    private ItemGenerator _generator;
    private ModelCollection _modelCollection;

    public ItemGenerationPresenter(ItemGenerator generator, ModelCollection modelCollection)
    {
        _generator = generator;

        _modelCollection = modelCollection;
    }

    private void GenerateItems()
    {
        InventoryModel inventory = _modelCollection.GetInventory(InventoryType.INVENTARY);
        List<ItemSO> allowedItems = _generator.GetAllowedItems(3, inventory);
        for (int i = 0; i < allowedItems.Count; i++)
        {
            ItemSO itemSO = allowedItems[i];

            ItemModel item = _generator.CreateItem(itemSO.type, itemSO.GetStructure());

            InventoryModel caseInventory = _modelCollection.GetInventory(InventoryType.CASE);
            caseInventory.TryAddItem(item, new(i * 2, 0));
        }
    }

    private void OnItemAddedToInventory(ItemModel item, (int row, int col) position)
    {
        InventoryModel caseInventory = _modelCollection.GetInventory(InventoryType.CASE);
        if (caseInventory.IsEmpty())
        {
            GenerateItems();
        }
    }

    public void Enable()
    {
        InventoryModel inventory = _modelCollection.GetInventory(InventoryType.INVENTARY);
        inventory.ItemAdded += OnItemAddedToInventory;

        GenerateItems();
    }

    public void Disable()
    {
        InventoryModel inventory = _modelCollection.GetInventory(InventoryType.INVENTARY);
        inventory.ItemAdded -= OnItemAddedToInventory;
    }
}
