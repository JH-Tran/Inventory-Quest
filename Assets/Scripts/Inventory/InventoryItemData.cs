using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    public string id;
    public string displayName;
    [TextArea(3, 10)]
    public string description;
    public Sprite icon;
    public int width = 1;
    public int height = 1;
    public ItemClassification itemClass = new ItemClassification();
}

public enum ItemClassification
{
    consumableItem,
    headItem,
    chestItem,
    legItem,
    weaponItem,
    anyItem,
    moveItem
};
public enum WeaponClassification
{
    none,
    staff,
    bow,
    sword,
};
