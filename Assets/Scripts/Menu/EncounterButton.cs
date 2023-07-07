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
    private int requirementBoss = 10;

    public void Update()
    {
        encounterButton.interactable = playerUnitInstance.currentUnitHealth > 0 ? true : false;
        encounterButton.interactable = encounterSystem.EnemiesDefeated > requirementBoss ? true : false;
    }

    public void OnButtonEcounter()
    {
        encounterSystem.RandomEncounter();
    }
}
