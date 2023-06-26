using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    public Text nameText;
    public Slider hpSlider;
    public Image unitImage;
    public Text levelText;
    public Text healthText;
    public Text statusText;

    public void SetHUD(UnitInstance unit)
    {
        nameText.text = unit.unitData.displayName;
        levelText.text = $"Lv.{unit.Level}";
        UpdateStatusEffect(unit);
        unitImage.sprite = unit.unitData.unitSprite;
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentUnitHealth;
        healthText.text = $"{unit.currentUnitHealth}/{unit.maxHp}hp";
    }
    public void SetHP(UnitInstance unit)
    {
        hpSlider.value = unit.currentUnitHealth;
        healthText.text = $"{unit.currentUnitHealth}/{unit.maxHp}hp";
    }
    public void UpdateStatusEffect(UnitInstance unit)
    {
        if (unit.GetUnitStatusEffect() != StatusEffect.NONE)
        {
            statusText.text = $"{unit.GetUnitStatusEffect()}";
        }
        else
        {
            statusText.text = "";
        }
    }
}
