using System.Linq;

[System.Serializable]
public class ItemSettings
{
    public ItemSO[] items;

    public ItemSO GetItemSoByType(ItemType type)
    {
        return items.First(i => i.type == type);
    }
}