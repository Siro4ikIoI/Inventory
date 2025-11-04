using UnityEngine;

[System.Serializable]
public class ItemSettings
{
    public int itemID;
    public GameObject item;
    public Row[] block;
}

[System.Serializable]
public class Row
{
    public int[] block_m;
}