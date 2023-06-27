using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveItem : MonoBehaviour
{
    public MovesData moveData;
    public InventoryItem inventoryItem;
    public TMPro.TextMeshProUGUI moveText;
    private Image moveBackground;

    public void Start()
    {
        moveBackground = GetComponent<Image>();
        moveText = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        RectTransform moveItemRectTransform = GetComponent<RectTransform>();
        moveItemRectTransform.localScale = new Vector2(1,1);
        if (moveText != null)
        {
            moveText.text = moveData.moveName;
        }
        switch (moveData.elementalType)
        {
            case ElementTypes.NORMAL:
                moveBackground.color = Color.white;
                break;
            case ElementTypes.FIRE:
                moveBackground.color = Color.red;
                break;
            case ElementTypes.WATER:
                moveBackground.color = Color.blue;
                break;
            case ElementTypes.GRASS:
                moveBackground.color = Color.green;
                break;
            case ElementTypes.DARK:
                moveBackground.color = Color.magenta;
                break;
            case ElementTypes.LIGHT:
                moveBackground.color = Color.yellow;
                break;
        }

    }
}
