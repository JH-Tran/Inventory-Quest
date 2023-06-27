using System.Collections;
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

    public void UpdatePlayerStatus()
    {
        headGrid.GetComponentInChildren<InventoryItemData>();
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
}
