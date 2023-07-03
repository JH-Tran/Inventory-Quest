using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryStatUserInterface : MonoBehaviour
{
    [SerializeField] UnitInstance playerUnitInstance;
    //Player Stat values
    [SerializeField] TMP_Text playerHealthTextMesh;
    [SerializeField] TMP_Text playerAttackTextMesh;
    [SerializeField] TMP_Text playerSpecialAttackTextMesh;
    [SerializeField] TMP_Text playerDefenceTextMesh;
    [SerializeField] TMP_Text playerSpecialDefenceTextMesh;
    [SerializeField] TMP_Text playerSpeedTextMesh;
    //About item information
    [SerializeField] GameObject itemGameObject;
    [SerializeField] TMP_Text itemName;
    [SerializeField] TMP_Text itemCategory;
    [SerializeField] TMP_Text itemAbout;
    public void UpdatePlayerStatUserInterface()
    {
        playerHealthTextMesh.text = $"Health: {playerUnitInstance.maxHp + playerUnitInstance.playerMaxHp}";
        playerAttackTextMesh.text = $"Attack: {playerUnitInstance.attack + playerUnitInstance.playerAttack}";
        playerSpecialAttackTextMesh.text = $"Special Attack: {playerUnitInstance.specialAttack + playerUnitInstance.playerSpecialAttack}";
        playerDefenceTextMesh.text = $"Defence: {playerUnitInstance.defence + playerUnitInstance.playerDefence}";
        playerSpecialDefenceTextMesh.text = $"Special Defence: {playerUnitInstance.specialDefence + playerUnitInstance.playerSpecialDefence}";
        playerSpeedTextMesh.text = $"Speed: {playerUnitInstance.unitData.speed + playerUnitInstance.playerSpeed}";
    }

    public void UpdateItemAboutUserInterface(InventoryItem inventoryItem)
    {
        itemCategory.text = $"{inventoryItem.data.itemClass}";
        itemAbout.text = "";
        if (inventoryItem.data.itemClass == ItemClassification.Move)
        {
            MoveItem moveItem = inventoryItem.gameObject.GetComponent<MoveItem>();
            itemName.text = $"{moveItem.moveData.moveName}";
            itemAbout.text = $"Element: {moveItem.moveData.elementalType}\n";
            if (moveItem.moveData.moveType == MoveType.PHYSICAL || moveItem.moveData.moveType == MoveType.SPECIAL)
            {
                itemAbout.text += $"Type: {moveItem.moveData.moveType}\n";
                itemAbout.text += $"Power: {moveItem.moveData.power}\n";
                itemAbout.text += $"Accuracy: {moveItem.moveData.accuracy}\n";
            }
            else
            {
                itemAbout.text += $"Type: {moveItem.moveData.moveType}\n";
                itemAbout.text += $"Status: {moveItem.moveData.statusEffect}\n";
                itemAbout.text += $"Status Accuracy: {moveItem.moveData.statusAccuracy}\n";
            }
        }
        else if (inventoryItem.data.itemClass == ItemClassification.Head || inventoryItem.data.itemClass == ItemClassification.Chest || inventoryItem.data.itemClass == ItemClassification.Leg || inventoryItem.data.itemClass == ItemClassification.Weapon)
        {
            itemName.text = $"{inventoryItem.data.displayName}";
            for (int i = 0; i < inventoryItem.gearStatList.Count; i++)
            {
                itemAbout.text += $"{inventoryItem.gearStatList[i].Stat}: {inventoryItem.gearStatList[i].StatValue}\n";
            }
        }
    }

    public void SetItemAboutUserInterface(bool isActive)
    {
        itemGameObject.SetActive(isActive);
    }
}
