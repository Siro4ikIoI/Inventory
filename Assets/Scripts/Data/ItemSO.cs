using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Game/Items")]
public class ItemSO : ScriptableObject
{
    public ItemSettings[] items;
}