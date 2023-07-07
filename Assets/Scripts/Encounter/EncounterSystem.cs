using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    public GameObject inventoryMenuGameObject;
    public InventoryButtonManager inventoryButtonManager;

    [SerializeField] private GameObject battleSystemGameObject;
    [SerializeField] private GameObject adventureMenuGameObject;
    [SerializeField] List<UnitData> commonEnemiesList;
    [SerializeField] List<UnitData> bossEnemiesList;
    [SerializeField] TMP_Text bossCounterText;
    [SerializeField] TMP_Text enemiesCounterText;
    [SerializeField] TMP_Text playerDeathText;
    private int bossDefeated = 0;
    private int enemiesDefeated = 0;
    private int playerDeath = 0;

    public int BossDefeated => bossDefeated;
    public int EnemiesDefeated => enemiesDefeated;

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

    private void Start()
    {
        UpdateAdventureStats();
    }

    public void RandomEncounter()
    {
        if (commonEnemiesList.Count <= 0) {
            Debug.LogError("Encounter List Empty!");
            return; 
        }
        int randomUnit = Random.Range(0, commonEnemiesList.Count);
        adventureMenuGameObject.SetActive(false);
        battleSystemGameObject.SetActive(true);
        battleSystemGameObject.GetComponent<BattleSystem>().StartBattle(commonEnemiesList[randomUnit]);
    }
    public void BossEncounter()
    {
        if (bossEnemiesList.Count <= 0)
        {
            Debug.LogError("Encounter List Empty!");
            return;
        }
        adventureMenuGameObject.SetActive(false);
        battleSystemGameObject.SetActive(true);
        battleSystemGameObject.GetComponent<BattleSystem>().StartBattle(commonEnemiesList[bossDefeated]);
    }
    public void AddBossDefeated()
    {
        bossDefeated++;
        UpdateAdventureStats();
    }
    public void AddEnemiesDefeated()
    {
        enemiesDefeated++;
        UpdateAdventureStats();
    }
    public void AddPlayerDeath()
    {
        playerDeath++;
        UpdateAdventureStats();
    }
    private void UpdateAdventureStats()
    {
        bossCounterText.text = $"Boss Defeated: {bossDefeated}";
        enemiesCounterText.text = $"Enemies Defeated: {enemiesDefeated}";
        playerDeathText.text = $"Player Deaths: {playerDeath}";
    }

}
