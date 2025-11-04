using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] ItemSO itemSO;
    [SerializeField] CanvasView canvasView;

    private void Start()
    {
        InventoryPresenter inventoryPresenter = new InventoryPresenter(canvasView);
        inventoryPresenter.Initialize(itemSO);
    }
}
