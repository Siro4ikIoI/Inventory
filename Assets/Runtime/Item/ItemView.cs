using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private RectTransform rectTransform;

    [SerializeField] private BlockGridView blocksWrapper;

    [SerializeField] private DragAndDropView dragAndDrop;
    public DragAndDropView DragAndDrop { get { return dragAndDrop; } }

    // Установка позиции итема
    public void SetPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public Vector2 GetPosition()
    {
        return rectTransform.anchoredPosition;
    }

    public void SetRotation(Direction direction, int[,] blocks)
    {
        blocksWrapper.Rotate(blocks);
        iconImage.transform.eulerAngles = Vector3.forward * -90 * (int)direction;
    }
}
