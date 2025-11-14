using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;
    [SerializeField] CanvasView canvasView;
    [SerializeField] ItemViewGenerator ItemViewGenerator;
    [SerializeField] InputHandler input;
    [SerializeField] ScoreView scoreView;

    private ModelCollection _modelCollection;

    private InventoryPresenter _inventoryPresenter;
    private InventoryPresenter _caseInventoryPresenter;
    private ItemGenerationPresenter _itemGenerationPresenter;
    private DragAndDropCreator _dragAndDropCreator;
    private ItemRotationPresenter _itemRotationPresenter;
    private InventoryHighlightPresenter _inventoryHighlightPresenter;
    private ScorePresenter _scorePresenter;

    private void Start()
    {
        _modelCollection = new ModelCollection();

        _inventoryPresenter = CreateInventoryPresenter(InventoryType.INVENTARY, new Pair(5, 9));
        _caseInventoryPresenter = CreateInventoryPresenter(InventoryType.CASE, new Pair(7, 4));

        ItemGenerator itemGenerator = new ItemGenerator(itemSO);
        _itemGenerationPresenter = new ItemGenerationPresenter(itemGenerator, ItemViewGenerator, _modelCollection, canvasView);

        _dragAndDropCreator = new DragAndDropCreator(_modelCollection, canvasView);
        _itemRotationPresenter = new ItemRotationPresenter(input, _modelCollection, canvasView);
        _inventoryHighlightPresenter = new InventoryHighlightPresenter(_modelCollection, canvasView);
        _scorePresenter = new ScorePresenter(new GameState(), scoreView, _modelCollection, canvasView);

        _inventoryPresenter.Enable();
        _caseInventoryPresenter.Enable();
        _inventoryHighlightPresenter.Enable();
        _dragAndDropCreator.Enable();
        _itemRotationPresenter.Enable();
        _scorePresenter.Enable();
        _itemGenerationPresenter.Enable();
    }

    private InventoryPresenter CreateInventoryPresenter(InventoryType inventoryType, Pair inventoryShape)
    {
        Inventory inventory = new Inventory(inventoryShape);
        _modelCollection.AddInventory(inventoryType, inventory);
        
        InventoryView inventoryView = canvasView.GetInventoryView(inventoryType);
        inventoryView.SetShape(inventory.Shape.Row, inventory.Shape.Col);
        
        return new InventoryPresenter(inventory, inventoryView, _modelCollection, canvasView);
    }

    private void OnDestroy()
    {
        _itemGenerationPresenter.Disable();
        _scorePresenter.Disable();
        _itemRotationPresenter.Disable();
        _dragAndDropCreator.Disable();
        _inventoryHighlightPresenter.Disable();
        _caseInventoryPresenter.Disable();
        _inventoryPresenter.Disable();
    }
}
