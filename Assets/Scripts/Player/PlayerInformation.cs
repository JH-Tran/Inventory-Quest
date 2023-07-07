using System.Collections.Generic;
using UnityEngine;

public class PlayerInformation : MonoBehaviour
{
    public UnitInstance playerUnitInstance;
    public List<GameObject> moveGameObjectList = new(4);

    public InventoryController inventoryController;

    public GameObject headGrid;
    public GameObject chestGrid;
    public GameObject legGrid;
    public GameObject weaponGrid;
    
    private bool isMoveInitalised = false;

    public void Start()
    {
        playerUnitInstance.Initalise();
    }
    public void OnButtonRestPlayer()
    {
        playerUnitInstance.currentUnitHealth = playerUnitInstance.maxHp;
        playerUnitInstance.RemoveUnitStatusEffect();
    }
    public void InitiatePlayerMoves()
    {
        if (!isMoveInitalised)
        {
            inventoryController.InsertPlayerInitalMoves(playerUnitInstance, moveGameObjectList);
            isMoveInitalised = true;
        }
    }
    public void SetPlayerMoves()
    {
        if (isMoveInitalised == false) { return; }
        for (int i = 0; i < playerUnitInstance.moveList.Count; i++)
        {
            try
            {
                playerUnitInstance.moveList[i] = moveGameObjectList[i].GetComponentInChildren<MoveItem>().moveData;
            }
            catch
            {
                Debug.Log($"Player move {i} is not assigned.");
                playerUnitInstance.moveList[i] = null;
            }
        }
    }
    public void SetPlayerGearStatus()
    {
        ResetPlayerStat();
        if (headGrid.GetComponentInChildren<InventoryItem>() != null)
        {
            InventoryItem headGear = headGrid.GetComponentInChildren<InventoryItem>();
            for (int i = 0; i < headGear.gearStatList.Count;i++)
            {
                UpdatePlayerStat(headGear.gearStatList[i].Stat, headGear.gearStatList[i].StatValue);
            }
        }
        if (chestGrid.GetComponentInChildren<InventoryItem>() != null)
        {
            InventoryItem chestGear = chestGrid.GetComponentInChildren<InventoryItem>();
            for (int i = 0; i < chestGear.gearStatList.Count; i++)
            {
                UpdatePlayerStat(chestGear.gearStatList[i].Stat, chestGear.gearStatList[i].StatValue);
            }
        }
        if (legGrid.GetComponentInChildren<InventoryItem>() != null)
        {
            InventoryItem legGear = legGrid.GetComponentInChildren<InventoryItem>();
            for (int i = 0; i < legGear.gearStatList.Count; i++)
            {
                UpdatePlayerStat(legGear.gearStatList[i].Stat, legGear.gearStatList[i].StatValue);
            }
        }
        if (weaponGrid.GetComponentInChildren<InventoryItem>() != null)
        {
            InventoryItem weaponGear = weaponGrid.GetComponentInChildren<InventoryItem>();
            for (int i = 0; i < weaponGear.gearStatList.Count; i++)
            {
                UpdatePlayerStat(weaponGear.gearStatList[i].Stat, weaponGear.gearStatList[i].StatValue);
            }
        }
        if (playerUnitInstance.currentUnitHealth > playerUnitInstance.maxHp)
        {
            playerUnitInstance.SetUnitHealthToMaxHealth();
        }
        Debug.Log($"PLAYER CURRENT HEALTH: {playerUnitInstance.currentUnitHealth}");
        Debug.Log($"HP: {playerUnitInstance.maxHp}, ATTACK: {playerUnitInstance.attack}, SpATTACK: {playerUnitInstance.playerSpecialAttack}, Defence: {playerUnitInstance.playerDefence}, SpDefence: {playerUnitInstance.playerSpecialDefence}, Speed: {playerUnitInstance.playerSpeed + playerUnitInstance.unitData.Speed}");
    }
    private void UpdatePlayerStat(UnitStats stat, int value)
    {
        switch (stat) 
        {
            case UnitStats.ATTACK:
                playerUnitInstance.playerAttack += value;
                break;
            case UnitStats.SPECIALATTACK:
                playerUnitInstance.playerSpecialAttack += value;
                break;
            case UnitStats.DEFENCE:
                playerUnitInstance.playerDefence += value;
                break;
            case UnitStats.SPECIALDEFENCE:
                playerUnitInstance.playerSpecialDefence += value;
                break;
            case UnitStats.HEALTH:
                playerUnitInstance.playerMaxHp += value;
                break;
            case UnitStats.SPEED:
                playerUnitInstance.playerSpeed += value;
                break;
        }
    }
    private void ResetPlayerStat()
    {
        playerUnitInstance.playerAttack = 0;
        playerUnitInstance.playerSpecialAttack = 0;
        playerUnitInstance.playerDefence = 0;
        playerUnitInstance.playerSpecialDefence = 0;
        playerUnitInstance.playerMaxHp = 0;
        playerUnitInstance.playerSpeed = 0;
    }
}
