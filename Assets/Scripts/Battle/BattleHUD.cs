using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Slider hpSlider;
    [SerializeField] Slider expSlider;
    [SerializeField] Image unitImage;
    [SerializeField] Text levelText;
    [SerializeField] Text healthText;
    [SerializeField] Text statusText;

    public void SetHUD(UnitInstance unit)
    {
        nameText.text = unit.unitData.DisplayName;
        SetLevel(unit);
        UpdateStatusEffect(unit);
        SetExperience(unit);
        SetHP(unit);
        unitImage.sprite = unit.unitData.UnitSprite;
    }
    public void SetHP(UnitInstance unit)
    {
        hpSlider.maxValue = unit.maxHp;
        hpSlider.value = unit.currentUnitHealth;
        healthText.text = $"{unit.currentUnitHealth}/{unit.maxHp}hp";
    }
    public void SetExperience(UnitInstance unit)
    {
        if (expSlider == null) return;
        float normalizeExperience = unit.GetNormalizeExperience();
        expSlider.normalizedValue = normalizeExperience;
    }
    public void SetLevel(UnitInstance unit)
    {
        levelText.text = $"Lv.{unit.Level}";
    }

    public IEnumerator SetExperienceSmooth (UnitInstance unit, bool reset = false)
    {
        if (expSlider == null) yield break;
        if (reset == true) { expSlider.normalizedValue = 0; }
        float normalizeExperience = unit.GetNormalizeExperience();
        yield return expSlider.DOValue(normalizeExperience, 1.5f).WaitForCompletion();
    }
    public void UpdateStatusEffect(UnitInstance unit)
    {
        if (unit.GetUnitStatusEffect() != StatusEffect.NONE)
        {
            if (unit.GetUnitStatusEffect() == StatusEffect.BURN)
            {
                statusText.text = $"BRN";
                statusText.color = Color.red;
            }
            else if (unit.GetUnitStatusEffect() == StatusEffect.PARALYSIS)
            {
                statusText.text = $"PAR";
                statusText.color = Color.yellow;
            }
            else if (unit.GetUnitStatusEffect() == StatusEffect.POISON)
            {
                statusText.text = $"PSN";
                statusText.color = Color.magenta;
            }
            else if (unit.GetUnitStatusEffect() == StatusEffect.FROZEN)
            {
                statusText.text = $"FZN";
                statusText.color = Color.blue;
            }
            else if (unit.GetUnitStatusEffect() == StatusEffect.SLEEP)
            {
                statusText.text = $"SLP";
                statusText.color = Color.grey;
            }
            else
            {
                statusText.text = "ERR";
                Debug.LogError($"Status text in {gameObject} display {unit.GetUnitStatusEffect()} has not been implemented.");
            }
        }
        else
        {
            statusText.text = "";
        }
    }
}
