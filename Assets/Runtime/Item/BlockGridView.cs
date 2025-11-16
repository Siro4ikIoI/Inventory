using UnityEngine;
using UnityEngine.UI;

public class BlockGridView : MonoBehaviour
{
    [SerializeField] private Color _blockColor;
    [SerializeField] private float _blockAlpha;

    [SerializeField] private GridLayoutGroup _grid;
    private Image[] _blocks;

    private void Awake()
    {
        Image[] children = GetComponentsInChildren<Image>();
        _blocks = new Image[children.Length - 1];
        for (int i = 0; i < _blocks.Length; i++)
        {
            _blocks[i] = children[i];
        }
    }

    public void Rotate(int[,] blockStucture)
    {
        _grid.constraintCount = blockStucture.GetLength(0);

        int i = 0;
        foreach (var value in blockStucture)
        {
            Color color = _blockColor;
            color.a = _blockAlpha * value;
            _blocks[i].color = color;
            _blocks[i].raycastTarget = value == 1;
            i++;
        }
    }
}
