using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    private Color normalColor = Color.white;
    private Color validColor = Color.green;
    private Color invalidColor = Color.red;

    void Awake()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }

        if (backgroundImage != null)
        {
            normalColor = backgroundImage.color;
        }
    }

    public void SetHighlight(int state)
    {
        if (backgroundImage == null) return;

        switch (state)
        {
            case 0:
                backgroundImage.color = normalColor;
                break;
            case 1:
                backgroundImage.color = validColor;
                break;
            case 2:
                backgroundImage.color = invalidColor;
                break;
            default:
                backgroundImage.color = normalColor;
                break;
        }
    }

    public void ResetHighlight()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = normalColor;
        }
    }
}