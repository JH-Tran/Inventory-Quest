using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(GridManager))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler 
{
    InventoryController inventoryController;
    GridManager gridManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemGridManager = gridManager;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGridManager = null;
    }

    private void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        gridManager = GetComponent<GridManager>();
    }
}
