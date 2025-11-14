using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;

    private Dictionary<CellViewState, Color> stateToColor = new()
    {
        { CellViewState.NORMAL, Color.white },
        { CellViewState.VALID, Color.green },
        { CellViewState.INVALID, Color.red }
    };

    public void SetHighlight(int state)
    {
        _backgroundImage.color = stateToColor[(CellViewState)state];
    }

    public void ResetHighlight()
    {
        _backgroundImage.color = stateToColor[CellViewState.NORMAL];
    }
}
