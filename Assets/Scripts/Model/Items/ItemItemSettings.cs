using UnityEngine;

public class ItemSettings : MonoBehaviour
{
    public Row[] block;

    private void Start()
    {
        GetBlock(0, 0);
    }

    private int GetBlock(int n, int m)
    {
        return block[n].block_m[m];
    }
}

[System.Serializable]
public class Row
{
    public int[] block_m;
}