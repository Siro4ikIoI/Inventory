using UnityEngine;

[CreateAssetMenu(fileName = "Items", menuName = "Game/Items")]
public class ItemSO : ScriptableObject
{
    public ItemType type;
    public ItemView item;
    public Row[] row;

    public int[,] GetStructure()
    {
        int[,] structure = new int[row.Length, row[0].col.Length];
        for (int i = 0; i < row.Length; i++)
        {
            for (int j = 0; j < row[i].col.Length; j++)
            {
                structure[i, j] = row[i].col[j];
            }
        }
        return structure;
    }
}

[System.Serializable]
public class Row
{
    public int[] col;
}