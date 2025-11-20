using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private RectTransform _rectTransform;

    [SerializeField] private BlockGridView _blocksWrapper;

    [SerializeField] private DragAndDropView _dragAndDrop;
    public DragAndDropView DragAndDrop { get { return _dragAndDrop; } }

    public void SetPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    public Vector2 GetPosition()
    {
        return _rectTransform.anchoredPosition;
    }

    public void SetRotation(Direction direction, int[,] blocks)
    {
        _blocksWrapper.Rotate(blocks);
        _iconImage.transform.eulerAngles = Vector3.forward * -90 * (int)direction;
    }
}
