using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryButtonManager : MonoBehaviour
{
    //Main menu
    public GameObject mainMenu;
    public GameObject inventoryMenu;
    //Gameobject that changes based on whether player is in battle
    public GameObject adventureAfterBattleButton;
    public GameObject battleBackButton;
    public GameObject mainMenuButton;
    //
    public GameObject gearInventory;
    public GameObject movesInventory;

    public GameObject dropGridGameObject;

    [SerializeField] InventoryStatUserInterface inventoryStatUserInterface;

    private void DefaultInventoryLayout()
    {
        gearInventory.SetActive(true);
        movesInventory.SetActive(false);
    }
    public void OpenInventoryAfterBattle()
    {
        adventureAfterBattleButton.SetActive(true);
        battleBackButton.SetActive(false);
        mainMenuButton.SetActive(false);
        inventoryStatUserInterface.UpdatePlayerStatUserInterface();
        DefaultInventoryLayout();
    }
    private void ReturnToMainMenu()
    {
        DefaultInventoryLayout();
        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
    }
    public void OnButtonReturnToMainMenu()
    {
        ReturnToMainMenu();
    }
    public void OnButtonClearDropGrid()
    {
        dropGridGameObject.GetComponent<GridManager>().ClearGrid();
        foreach (Transform item in dropGridGameObject.transform)
        {
            if (!item.CompareTag("Highlighter"))
            {
                GameObject.Destroy(item.gameObject);
            }
        }
    }
    public void OnButtonOpenGear()
    {
        gearInventory.SetActive(true);
        movesInventory.SetActive(false);
    }
    public void OnButtonOpenMoves()
    {
        gearInventory.SetActive(false);
        movesInventory.SetActive(true);
    }
    public void OnButtonInventoryFromMainMenu()
    {
        battleBackButton.SetActive(false);
        adventureAfterBattleButton.SetActive(false);
        mainMenuButton.SetActive(true);
        inventoryStatUserInterface.UpdatePlayerStatUserInterface();
        DefaultInventoryLayout();
    }
    public void OnButtonInventoryFromBattle()
    {
        battleBackButton.SetActive(true);
        mainMenuButton.SetActive(false);
        adventureAfterBattleButton.SetActive(false);
        inventoryStatUserInterface.UpdatePlayerStatUserInterface();
        DefaultInventoryLayout();
    }
}
