using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncounterButton : MonoBehaviour
{
    [SerializeField] EncounterSystem encounterSystem;
    [SerializeField] Button encounterButton;
    [SerializeField] Button bossEncounterButton;
    [SerializeField] UnitInstance playerUnitInstance;
    private int requirementBoss = 5;

    public void Update()
    {
        if (playerUnitInstance.currentUnitHealth <= 0)
        {
            encounterButton.interactable = false;
            bossEncounterButton.interactable = false;
        }
        else
        {
            encounterButton.interactable = true;
            bossEncounterButton.interactable = encounterSystem.EnemiesDefeated >= requirementBoss ? true : false;
        }
    }

    public void OnButtonEcounter()
    {
        encounterSystem.RandomEncounter();
    }
}
