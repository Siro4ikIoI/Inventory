using System.Collections.Generic;

public class ItemGenerationPresenter
{
    private ItemGenerator _generator;
    private ItemViewGenerator _viewGenerator;

    private ModelCollection _modelCollection;
    private CanvasView _canvasView;

    public ItemGenerationPresenter(ItemGenerator generator, ItemViewGenerator viewGenerator,
                                ModelCollection modelCollection, CanvasView canvasView)
    {
        _generator = generator;
        _viewGenerator = viewGenerator;

        _modelCollection = modelCollection;
        _canvasView = canvasView;
    }

    private void GenerateItems()
    {
        Inventory inventory = _modelCollection.GetInventory(InventoryType.INVENTARY);
        List<ItemSettings> allowedItems = _generator.GetAllowedItems(3, inventory);
        for (int i = 0; i < allowedItems.Count; i++)
        {
            ItemSettings itemSetting = allowedItems[i];

            Item item = _generator.CreateItem(itemSetting.GetStructure());
            ItemView itemView = _viewGenerator.CreateItemView(itemSetting.item, _canvasView.transform);

            _canvasView.AddItemView(item.Id, itemView);
            _modelCollection.AddItem(item.Id, item);

            ItemPresenter itemPresenter = new ItemPresenter(item, itemView, _modelCollection, _canvasView);
            itemPresenter.Enable();

            Inventory caseInventory = _modelCollection.GetInventory(InventoryType.CASE);
            caseInventory.TryAddItem(item, new Pair(i * 2, 0));
        }
    }

    private void OnItemAddedToInventory(Item item, Pair position)
    {
        Inventory caseInventory = _modelCollection.GetInventory(InventoryType.CASE);
        if (caseInventory.IsEmpty())
        {
            GenerateItems();
        }
    }

    public void Enable()
    {
        Inventory inventory = _modelCollection.GetInventory(InventoryType.INVENTARY);
        inventory.ItemAdded += OnItemAddedToInventory;

        GenerateItems();
    }

    public void Disable()
    {
        Inventory inventory = _modelCollection.GetInventory(InventoryType.INVENTARY);
        inventory.ItemAdded -= OnItemAddedToInventory;
    }
}
