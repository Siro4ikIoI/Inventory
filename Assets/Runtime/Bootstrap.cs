using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [Header("Inventories")]
    [SerializeField] InventoryView inventory;
    [SerializeField] InventoryView caseInventory;

    [Header("Item Settings")]
    [SerializeField] ItemSettings itemSettings;

    [Header("Input")]
    [SerializeField] InputHandler input;

    [Header("UI")]
    [SerializeField] ScoreView scoreView;

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

        _inventoryPresenter = CreateInventoryPresenter(InventoryType.INVENTARY, new Pair(5, 9), inventory);
        _caseInventoryPresenter = CreateInventoryPresenter(InventoryType.CASE, new Pair(7, 4), caseInventory);

        ItemGenerator itemGenerator = new ItemGenerator(itemSettings);
        _itemGenerationPresenter = new ItemGenerationPresenter(itemGenerator, _modelCollection);

        _itemRotationPresenter = new ItemRotationPresenter(input, _modelCollection);
        _inventoryHighlightPresenter = new InventoryHighlightPresenter(_modelCollection);
        _scorePresenter = new ScorePresenter(new GameState(), scoreView, _modelCollection);

        _inventoryPresenter.Enable();
        _caseInventoryPresenter.Enable();
        _inventoryHighlightPresenter.Enable();
        _itemRotationPresenter.Enable();
        _scorePresenter.Enable();
        _itemGenerationPresenter.Enable();
    }

    private InventoryPresenter CreateInventoryPresenter(InventoryType inventoryType, Pair inventoryShape, InventoryView inventoryView)
    {
        Inventory inventory = new Inventory(inventoryType, inventoryShape);
        _modelCollection.AddInventory(inventoryType, inventory);

        inventoryView.SetShape(inventory.Shape.Row, inventory.Shape.Col);
        
        return new InventoryPresenter(inventory, inventoryView, _modelCollection, itemSettings);
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
