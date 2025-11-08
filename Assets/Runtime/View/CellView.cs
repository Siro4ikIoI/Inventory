using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CellState :int
{
    NORMAL = 0,
    VALID = 1,
    INVALID = 2
}

public class CellView : MonoBehaviour
{
    [SerializeField] private Image backgroundImage; 

    private Dictionary<CellState, Color> stateToColor = new()
    {
        { CellState.NORMAL, Color.white },
        { CellState.VALID, Color.green },
        { CellState.INVALID, Color.red }
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

        backgroundImage.color = stateToColor[(CellState)state];
    }

    public void ResetHighlight()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = stateToColor[CellState.NORMAL];
        }
    }
}