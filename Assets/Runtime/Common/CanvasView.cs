using System.Collections.Generic;
using UnityEngine;

public class CanvasView : MonoBehaviour
{
    [SerializeField] private InventoryView inventoryView;
    [SerializeField] private InventoryView caseView;
    [SerializeField] private ScoreView scoreView;

    [SerializeField] private Canvas canvas;
    public Canvas Canvas { get { return canvas; } }

    private Dictionary<InventoryType, InventoryView> inventories = new();
    public InventoryView GetInventoryView(InventoryType inventoryType)
    {
        return inventories[inventoryType];
    }

    public IEnumerable<KeyValuePair<InventoryType, InventoryView>> GetAllInventoryViews()
    {
        return inventories;
    }

    private Dictionary<int, ItemView> spawnedItems = new Dictionary<int, ItemView>();
    public void AddItemView(int id, ItemView itemView)
    {
        spawnedItems.Add(id, itemView);
    }
    public ItemView GetItemView(int id)
    {
        return spawnedItems[id];
    }

    private void Awake()
    {
        inventories.Add(InventoryType.CASE, caseView);
        inventories.Add(InventoryType.INVENTARY, inventoryView);
    }
}
