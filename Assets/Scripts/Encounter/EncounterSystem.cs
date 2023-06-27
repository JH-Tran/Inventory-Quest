using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    public GameObject inventoryMenuGameObject;
    public InventoryButtonManager inventoryButtonManager;

    [SerializeField] private GameObject battleSystemGameObject;
    [SerializeField] private GameObject adventureMenuGameObject;
    [SerializeField] List<UnitData> unitList;

    #region Inventory Button
    public void OpenInventoryButton()
    {
        inventoryMenuGameObject.SetActive(true);
        battleSystemGameObject.SetActive(false);
        adventureMenuGameObject.SetActive(false);
        inventoryButtonManager.OnButtonInventoryFromMainMenu();
    }
    public void OpenAdventureMenu()
    {
        inventoryMenuGameObject.SetActive(false);
        battleSystemGameObject.SetActive(false);
        adventureMenuGameObject.SetActive(true);
    }
    #endregion

    public void RandomEncounter()
    {
        if (unitList.Count <= 0) {
            Debug.LogError("Encounter List Empty!");
            return; 
        }
        int randomUnit = Random.Range(0, unitList.Count);
        adventureMenuGameObject.SetActive(false);
        battleSystemGameObject.SetActive(true);
        battleSystemGameObject.GetComponent<BattleSystem>().StartBattle(unitList[randomUnit]);
    }
}
