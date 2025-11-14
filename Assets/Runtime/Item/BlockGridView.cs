using UnityEngine;
using UnityEngine.UI;

public class BlockGridView : MonoBehaviour
{
    [SerializeField] Color blockColor;
    [SerializeField] float blockAlpha;

    private GridLayoutGroup grid;
    private Image[] blocks;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();

        Image[] children = GetComponentsInChildren<Image>();
        blocks = new Image[children.Length - 1];
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i] = children[i];
        }
    }

    public void Rotate(int[,] blockStucture)
    {
        grid.constraintCount = blockStucture.GetLength(0);

        int i = 0;
        foreach(var value in blockStucture)
        {
            Color color = blockColor;
            color.a = blockAlpha * value;
            blocks[i].color = color;
            blocks[i].raycastTarget = value == 1;
            i++;
        }
    }
}
