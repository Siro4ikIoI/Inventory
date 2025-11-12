using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image backgroundImage; 

    private Dictionary<CellViewState, Color> stateToColor = new()
    {
        { CellViewState.NORMAL, Color.white },
        { CellViewState.VALID, Color.green },
        { CellViewState.INVALID, Color.red }
    };

    void Awake()
    {
        if (backgroundImage == null)
        {
            backgroundImage = GetComponent<Image>();
        }
    }

    public void SetHighlight(int state)
    {
        if (backgroundImage == null) return;

        backgroundImage.color = stateToColor[(CellViewState)state];
    }

    public void ResetHighlight()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = stateToColor[CellViewState.NORMAL];
        }
    }
}