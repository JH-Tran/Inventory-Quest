using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveItem : MonoBehaviour
{
    public MovesData moveData { get; set; }
    public InventoryItem inventoryItem;
    
    public TMPro.TextMeshProUGUI moveText;

    public void Start()
    {
        moveText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (moveText != null)
        {
            moveText.text = moveData.moveName;
        }
    }
}
