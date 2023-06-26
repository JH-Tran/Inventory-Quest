using System.Collections.Generic;
using UnityEngine;

public class UnitInstance : MonoBehaviour
{
    public UnitData unitData;

    [SerializeField]
    public int currentUnitHealth;
    private int level = 1;
    public int Level => level;

    [SerializeField]private StatusEffect unitStatusEffect = StatusEffect.NONE;
    public int turnDownCount = 0;
    public int poisonCount = 0;
    private string statusEffectString = "";
    //Static variables for
    private const int sleepMaxTurnCount = 3;
    private const int wakeUpChance = 25;
    private const int frozenMaxTurnCount = 5;
    private const int thawedChance = 10;
    private const int paralysisPassTurnChance = 25;
    private const int poisonCountMax = 3;
    private const float poisonDamageMultiplier = 0.0625f;
    private const float burnDamageMultiplier = 0.0625f;

    public List<MovesData> moveList = new(4);

    private float superEffectiveMultiplier = 1.5f;
    private float notSuperEffectiveMultiplier = 0.5f;
    private float normalEffectiveMultiplier = 1f;

    private int criticalRate = 200;
    private int maxCriticalChance = 1000;
    private float criticalMultiplier = 1.5f;
    private bool hitCriticalAttack;

    public void Awake()
    {
        if (unitData != null) 
        {
            level = unitData.startingLevel;
            currentUnitHealth = maxHp;
        }
    }
    public int attack
    {
        get { return Mathf.FloorToInt((unitData.attack * level) / 100f) + 5; }
    }
    public int defence
    {
        get { return Mathf.FloorToInt((unitData.defence * level) / 100f) + 5; }
    }
    public int specialAttack
    {
        get { return Mathf.FloorToInt((unitData.specialAttack * level) / 100f) + 5; }
    }
    public int spDefence
    {
        get { return Mathf.FloorToInt((unitData.specialDefence * level) / 100f) + 5; }
    }
    public int maxHp
    {
        get { return Mathf.FloorToInt((unitData.maxHealthPoint * level) / 100f) + 5; }
    }
    public void SetUnitHealth()
    {
        level = unitData.startingLevel;
        currentUnitHealth = maxHp;
    }
    public bool IsUnitCriticalAttack()
    {
        return hitCriticalAttack;
    }
    public void HealUnit(int heal)
    {
        currentUnitHealth += heal;
        if (currentUnitHealth > maxHp)
        {
            currentUnitHealth = maxHp;
        }
    }
    public StatusEffect GetUnitStatusEffect()
    {
        return unitStatusEffect;
    }
    public void CleanseUnitStatusEffect()
    {
        unitStatusEffect = StatusEffect.NONE;
    }
    public bool ApplyStatusEffect(UnitInstance unitTakingStatusEffect, ElementTypes moveElement, StatusEffect statusEffect, int accuarcy)
    {
        if (unitTakingStatusEffect.unitStatusEffect != StatusEffect.NONE)
        {
            return false;
        }
        else
        {
            if (RandomRoller(100, accuarcy))
            {
                unitTakingStatusEffect.unitStatusEffect = statusEffect;
                //Debug.Log($"{statusEffect} has applied to {unitTakingStatusEffect.name} with a {accuarcy}% chance.");
                if (unitTakingStatusEffect.unitStatusEffect == StatusEffect.FROZEN || unitTakingStatusEffect.unitStatusEffect == StatusEffect.SLEEP)
                {
                    unitTakingStatusEffect.turnDownCount = 0;
                }
                return true;
            }
            //Debug.Log($"{statusEffect} missed {unitTakingStatusEffect.name} with a {accuarcy}% chance.");
            return false;
        }        
    }
    //Status Effect for Burn and poison
    //Return true/false if the unit is dead
    public bool IsUnitDeadFromStatusEffect()
    {
        if (unitStatusEffect == StatusEffect.NONE)
        {
            return false;
        }
        int tickDamage = 1;
        switch (unitStatusEffect)
        {
            case StatusEffect.BURN:
                statusEffectString = $"{unitData.displayName} suffered from burning!";
                Debug.Log($"BURN MESSAGE: {statusEffectString}");
                tickDamage = Mathf.FloorToInt(maxHp * burnDamageMultiplier);
                if (tickDamage < 1)
                    tickDamage = 1;
                currentUnitHealth -= tickDamage;
                Debug.Log($"BurnDamage: {maxHp * burnDamageMultiplier}");
                break;
            case StatusEffect.POISON:
                if (poisonCount < poisonCountMax)
                {
                    poisonCount++;
                }
                if (poisonCount == 1)
                    statusEffectString = $"{unitData.displayName} suffered from poison!";
                else
                    statusEffectString = $"{unitData.displayName} suffered from poison badly!";
                tickDamage = Mathf.FloorToInt(poisonCount * maxHp * poisonDamageMultiplier);
                if (tickDamage < 1)
                    tickDamage = 1;
                currentUnitHealth -= tickDamage;
                Debug.Log($"PoisonCount: {poisonCount} || PoisonDamage: {tickDamage}");
                break;
        }
        if (currentUnitHealth <= 0)
            return true;
        else
            return false;
    }
    //Status effect for frozen, sleeping and paralysis
    public bool IsStatusEffectPassUnitTurn()
    {
        if ((unitStatusEffect == StatusEffect.SLEEP && turnDownCount < sleepMaxTurnCount) || (unitStatusEffect == StatusEffect.FROZEN && turnDownCount < frozenMaxTurnCount))
        {
            if ((unitStatusEffect == StatusEffect.SLEEP && RandomRoller(100, wakeUpChance)) || (unitStatusEffect == StatusEffect.FROZEN && RandomRoller(100, thawedChance)))
            {
                unitStatusEffect = StatusEffect.NONE;
                statusEffectString = $"{unitData.displayName} woke up!";
                return false;
            }
            if (unitStatusEffect == StatusEffect.SLEEP)
                statusEffectString = $"{unitData.displayName} is still asleep!";
            else if (unitStatusEffect == StatusEffect.FROZEN)
                statusEffectString = $"{unitData.displayName} is still frozen!";
            turnDownCount += 1;
            Debug.Log($"{unitStatusEffect} TURN COUNT: {turnDownCount}");
            return true;
        }
        else if (unitStatusEffect == StatusEffect.PARALYSIS)
        {
            if (RandomRoller(100, paralysisPassTurnChance))
            {
                statusEffectString = $"{unitData.displayName} is unable to move!";
                return true;
            }
        }
        else if ((unitStatusEffect == StatusEffect.SLEEP && turnDownCount >= sleepMaxTurnCount) || (unitStatusEffect == StatusEffect.FROZEN && turnDownCount >= frozenMaxTurnCount))
        {
            unitStatusEffect = StatusEffect.NONE;
            if (unitStatusEffect == StatusEffect.SLEEP)
                statusEffectString = $"{unitData.displayName} woke up!";
            else if (unitStatusEffect == StatusEffect.FROZEN)
                statusEffectString = $"{unitData.displayName} thawed out!";
            return true;
        }
        statusEffectString = "";
        return false;
    }
    //Move for basic attack physically/special are implemented
    public string GetStatusEffectString()
    {
        Debug.Log($"STATUS EFFECT STRING: {statusEffectString}");
        return statusEffectString;
    }
    public bool PhysicalAttack(UnitInstance unitTakingDamage, ElementTypes moveElementType, int power)
    {
        return unitTakingDamage.TakePhysicalDamage(moveElementType, attack, level, power, CalculateDamageBurnReduction());
    }
    public bool SpecialAttack(UnitInstance unitTakingDamage, ElementTypes moveElementType, int power)
    {
        return unitTakingDamage.TakeSpecialDamage(unitData.displayName, moveElementType, specialAttack, level, power, CalculateDamageBurnReduction());
    }
    public bool TakePhysicalDamage(ElementTypes moveElementType, int attackerAttack, int attackerLevel, int movePower, float burnMultiplier)
    {
        float calculatedDamage = (((2 * attackerLevel / 5) + 2) * movePower * (attackerAttack / defence) / 50 + 2) * CalculateElementEffectiveMultiplier(moveElementType,unitData.type1) * CalculateCriticalMultiplier() * burnMultiplier;
        if (unitData.type2 != ElementTypes.NONE)
        {
            calculatedDamage *= CalculateElementEffectiveMultiplier(moveElementType, unitData.type2);
        }
        if (calculatedDamage > 0)
        {
            currentUnitHealth -= Mathf.FloorToInt (calculatedDamage);
            //Debug.Log($"{attackerName} has dealt {calculatedDamage} to {unitData.name}");
        }
        else
        {
            //Debug.Log($"{attackerName} has defended {calculatedDamage} from {unitData.name}");
        }

        if (currentUnitHealth <= 0)
        {
            currentUnitHealth = 0;
            return true;
        }
        else
            return false;
    }
    public bool TakeSpecialDamage(string attackerName, ElementTypes moveElementType, int specialAttack, int attackerLevel, int movePower, float burnMultiplier)
    {
        float calculatedDamage = (((2 * attackerLevel / 5) + 2) * movePower * (specialAttack / spDefence) / 50 + 2) * 
            CalculateElementEffectiveMultiplier(moveElementType, unitData.type1) * 
            CalculateCriticalMultiplier() * burnMultiplier;

        if (unitData.type2 != ElementTypes.NONE)
        {
            calculatedDamage *= CalculateElementEffectiveMultiplier(moveElementType, unitData.type2);
        }
        if (calculatedDamage >= 1)
        {
            currentUnitHealth -= Mathf.FloorToInt(calculatedDamage);
            //Debug.Log($"{attackerName} has dealt {Mathf.FloorToInt(calculatedDamage)} special damage to {unitData.name}");
        }
        else
        {
            //Debug.Log($"{attackerName} has defended {calculatedDamage} from {unitData.name}");
        }
        if (currentUnitHealth <= 0)
        {
            currentUnitHealth = 0;
            return true;
        }
        else
            return false;
    }
    private float CalculateDamageBurnReduction()
    {
        if(unitStatusEffect == StatusEffect.BURN)
        {
            return 0.5f;
        }
        return 1f;
    }
    public float CalculateElementEffectiveMultiplier(ElementTypes attackerType, ElementTypes defenderType)
    {
        if ((attackerType == ElementTypes.WATER && defenderType == ElementTypes.FIRE) || 
            (attackerType == ElementTypes.GRASS && defenderType == ElementTypes.WATER) || 
            (attackerType == ElementTypes.FIRE && defenderType == ElementTypes.GRASS) ||
            (attackerType == ElementTypes.LIGHT && defenderType == ElementTypes.DARK) ||
            (attackerType == ElementTypes.DARK && defenderType == ElementTypes.LIGHT))
        {
            return superEffectiveMultiplier;
        }
        else if ((attackerType == ElementTypes.FIRE && defenderType == ElementTypes.WATER) ||
            (attackerType == ElementTypes.WATER && defenderType == ElementTypes.GRASS) ||
            (attackerType == ElementTypes.GRASS && defenderType == ElementTypes.FIRE))
        {
            return notSuperEffectiveMultiplier;
        }
        //If the elements are the same or have neutral reactions
        return normalEffectiveMultiplier;
    }
    public float CalculateCriticalMultiplier()
    {
        if (RandomRoller(maxCriticalChance, criticalRate))
        {
            Debug.Log("CRIT!");
            hitCriticalAttack = true;
            return criticalMultiplier;
        }
        Debug.Log("NOT CRIT!");
        hitCriticalAttack = false;
        return 1;
    }
    private bool RandomRoller(int maxRange, int chanceRate)
    {
        int randomDigit = Random.Range(1, maxRange + 1);
        //Debug.Log($"RR: {randomDigit} || {chanceRate}");
        if (randomDigit <= chanceRate)
        {
            //Debug.Log("RR: hit");
            return true;
        }
        //Debug.Log("RR: missed");
        return false;
    }
}
