using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Inventories")]
    [SerializeField] private InventoryView _inventory;
    [SerializeField] private InventoryView _caseInventory;

    [Header("Item Settings")]
    [SerializeField] private ItemSettings _itemSettings;

    [Header("Input")]
    [SerializeField] private InputHandler _input;

    [Header("UI")]
    [SerializeField] private ScoreView _scoreView;

    private ModelCollection _modelCollection;

    private InventoryPresenter _inventoryPresenter;
    private InventoryPresenter _caseInventoryPresenter;
    private ItemGenerationPresenter _itemGenerationPresenter;
    private ItemRotationPresenter _itemRotationPresenter;
    private InventoryHighlightPresenter _inventoryHighlightPresenter;
    private ScorePresenter _scorePresenter;

    private void Start()
    {
        _modelCollection = new ModelCollection();
        _modelCollection.SetDragAndDropContainer(new DragAndDropContainer());

        _inventoryPresenter = CreateInventoryPresenter(InventoryType.INVENTARY, new(5, 9), _inventory);
        _caseInventoryPresenter = CreateInventoryPresenter(InventoryType.CASE, new(7, 4), _caseInventory);

        ItemGenerator itemGenerator = new ItemGenerator(_itemSettings);
        _itemGenerationPresenter = new ItemGenerationPresenter(itemGenerator, _modelCollection);

        _itemRotationPresenter = new ItemRotationPresenter(_input, _modelCollection);
        _inventoryHighlightPresenter = new InventoryHighlightPresenter(_modelCollection);
        _scorePresenter = new ScorePresenter(new GameState(), _scoreView, _modelCollection);

        _inventoryPresenter.Enable();
        _caseInventoryPresenter.Enable();
        _inventoryHighlightPresenter.Enable();
        _itemRotationPresenter.Enable();
        _scorePresenter.Enable();
        _itemGenerationPresenter.Enable();
    }

    private InventoryPresenter CreateInventoryPresenter(InventoryType inventoryType, (int row, int col) inventoryShape, InventoryView inventoryView)
    {
        InventoryModel inventory = new InventoryModel(inventoryType, inventoryShape);
        _modelCollection.AddInventory(inventoryType, inventory);

        inventoryView.SetShape(inventory.Shape.row, inventory.Shape.col);

        return new InventoryPresenter(inventory, inventoryView, _modelCollection, _itemSettings);
    }

    private void OnDestroy()
    {
        _itemGenerationPresenter.Disable();
        _scorePresenter.Disable();
        _itemRotationPresenter.Disable();
        _inventoryHighlightPresenter.Disable();
        _caseInventoryPresenter.Disable();
        _inventoryPresenter.Disable();
    }
}
