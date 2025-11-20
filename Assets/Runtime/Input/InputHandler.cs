using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action RightClick;

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            RightClick?.Invoke();
        }
    }
}
