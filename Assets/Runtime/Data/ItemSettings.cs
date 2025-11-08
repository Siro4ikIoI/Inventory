using UnityEngine;

[System.Serializable]
public class ItemSettings
{
    public int itemID;
    public ItemView item;
    public Row[] block;

    public int[,] GetStructure()
    {
        int[,] structure = new int[block.Length, block[0].block_m.Length];
        for(int i = 0; i < block.Length; i++)
        {
            for (int j = 0; j < block[i].block_m.Length; j++)
            {
                structure[i, j] = block[i].block_m[j];
            }
        }
        return structure;
    }
}

[System.Serializable]
public class Row
{
    public int[] block_m;
}