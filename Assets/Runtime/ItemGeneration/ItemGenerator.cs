using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemGenerator
{
    private ItemSO _itemSO;

    private int _id = 0;

    public ItemGenerator(ItemSO itemSO)
    {
        _itemSO = itemSO;
    }

    public Item CreateItem(int[,] blocks)
    {
        _id++;
        return new Item(_id, blocks);
    }

    public List<ItemSettings> GetAllowedItems(int count, Inventory inventory)
    {
        List<ItemSettings> itemSettings = new();

        Matrix workingMatrix = new Matrix(inventory.ToMatrix().GetStructure());

        for (int i = 0; i < count; i++)
        {
            bool placed = false;
            List<ItemSettings> items = _itemSO.items.OrderBy(_ => Random.value).ToList();
                        
            for (int attempt = 0; attempt < items.Count && !placed; attempt++)
            {
                ItemSettings itemSetting = items[attempt];

                Matrix itemMatrix = new Matrix(itemSetting.GetStructure());

                if (TryFindFreePosition(itemMatrix, workingMatrix, out var actualPos))
                {
                    itemSettings.Add(itemSetting);

                    itemMatrix.Reshape(workingMatrix.Size, out itemMatrix, actualPos.Row, actualPos.Col);
                    workingMatrix = workingMatrix.Add(itemMatrix);

                    placed = true;
                }                
            }
        }

        return itemSettings;
    }

    private bool TryFindFreePosition(Matrix item, Matrix inventory, out Pair foundPosition)
    {
        foundPosition = new Pair(0, 0);

        for (int row = 0; row < inventory.Size.Row; row++)
        {
            for (int col = 0; col < inventory.Size.Col; col++)
            {
                bool inBounds = item.Reshape(inventory.Size, out Matrix itemMatrix, row, col);
                if (!inBounds)
                    continue;

                Matrix newCells = inventory.Add(itemMatrix);
                if (newCells.Max() <= (int)CellType.FILL)
                {
                    foundPosition = new Pair(row, col);
                    return true;
                }
            }
        }
        return false;
    }
}
