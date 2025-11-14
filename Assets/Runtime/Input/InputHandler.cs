using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action RightClick;

    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            RightClick?.Invoke();
        }
    }
}
