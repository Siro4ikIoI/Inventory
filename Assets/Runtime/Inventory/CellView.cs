using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellView : MonoBehaviour
{
    [SerializeField] private Image _backgroundImage;

    private Dictionary<CellViewState, Color> _stateToColor = new()
    {
        { CellViewState.NORMAL, Color.white },
        { CellViewState.VALID, Color.green },
        { CellViewState.INVALID, Color.red }
    };

    public void SetHighlight(int state)
    {
        _backgroundImage.color = _stateToColor[(CellViewState)state];
    }

    public void ResetHighlight()
    {
        _backgroundImage.color = _stateToColor[CellViewState.NORMAL];
    }
}
