using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[SerializeField]
public class InventoryItem : MonoBehaviour
{
    public InventoryItemData data { get; private set; }
    public int stackSize { get; private set; }
    public int onGridPositionX;
    public int onGridPositionY;
    public bool isRotated;
    public int HEIGHT { 
        get
        {
            if (isRotated == false)
            {
                return data.height;
            }
            return data.width;
        }
    }
    public int WIDTH
    {
        get
        {
            if (isRotated == false)
            {
                return data.width;
            }
            return data.height;
        }
    }

    public InventoryItem(InventoryItemData source)
    {
        data = source;
    }

    internal void Set(InventoryItemData inventoryItemData)
    {
        Debug.Log($"Created {inventoryItemData.name}");
        data = inventoryItemData;
        GetComponent<Image>().sprite = inventoryItemData.icon;
        Vector2 size = new Vector2();
        size.x = data.width * GridManager.tileSizeWidth;
        size.y = data.height * GridManager.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    internal void Rotate()
    {
        isRotated = !isRotated;
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, isRotated == true ? 90f : 0f);
    }
}
