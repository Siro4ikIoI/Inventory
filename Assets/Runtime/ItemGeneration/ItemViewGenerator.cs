using UnityEngine;

public class ItemViewGenerator : MonoBehaviour
{
    public ItemView CreateItemView(ItemView prefab, Transform parent)
    {
        return Instantiate<ItemView>(prefab, parent);
    }
}
