using System.Collections.Generic;
using UnityEngine;

public class UnitInstance : MonoBehaviour
{
    public UnitData unitData;

    [SerializeField]
    public int currentUnitHealth = 1;
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

    public List<InventoryItem> itemDropList;

    private float superEffectiveMultiplier = 1.5f;
    private float notSuperEffectiveMultiplier = 0.5f;
    private float normalEffectiveMultiplier = 1f;
    private ElementEffectiveness moveEffectivenessType1 = ElementEffectiveness.NONE;
    private ElementEffectiveness moveEffectivenessType2 = ElementEffectiveness.NONE;

    private int criticalRate = 200;
    private int maxCriticalChance = 1000;
    private float criticalMultiplier = 1.5f;
    private bool isDefenderTakeCriticalHit;
    public int attack
    {
        get { return (Mathf.FloorToInt((unitData.Attack * level) / 100f) + 5) + _playerAttack; }
    }
    public int defence
    {
        get { return (Mathf.FloorToInt((unitData.Defence * level) / 100f) + 5) + _playerDefence; }
    }
    public int specialAttack
    {
        get { return (Mathf.FloorToInt((unitData.SpecialAttack * level) / 100f) + 5) + _playerSpecialAttack
                
                ; }
    } 
    public int specialDefence
    {
        get { return (Mathf.FloorToInt((unitData.SpecialDefence * level) / 100f) + 5) + playerSpecialDefence; }
    }
    public int maxHp
    {
        get { return (Mathf.FloorToInt((unitData.MaxHealthPoint * level) / 100f) + 5) + _playerMaxHp; }
    }

    private int _playerAttack = 0;
    private int _playerDefence = 0;
    private int _playerSpecialAttack = 0;
    private int _playerSpecialDefence = 0;
    private int _playerMaxHp = 0;
    private int _playerSpeed;
    public int playerAttack { get => _playerAttack; set { _playerAttack = value; } }
    public int playerDefence { get => _playerDefence; set { _playerDefence = value; } }
    public int playerSpecialAttack { get => _playerSpecialAttack; set { _playerSpecialAttack = value; } }
    public int playerSpecialDefence { get => _playerSpecialDefence; set { _playerSpecialDefence = value; } }
    public int playerMaxHp { get => _playerMaxHp; set { _playerMaxHp = value; } }
    public int playerSpeed { get => _playerSpeed; set { _playerSpeed = value; } }

    public void Start()
    {
        if (unitData != null)
        {
            level = unitData.StartingLevel;
            currentUnitHealth = maxHp;
        }
    }
    public void SetUnitHealth()
    {
        level = unitData.StartingLevel;
        currentUnitHealth = maxHp;
    }
    public void SetUnitHealthToMaxHealth()
    {
        currentUnitHealth = maxHp;
    }
    public bool IsUnitCriticalAttack()
    {
        return isDefenderTakeCriticalHit;
    }
    public void HealUnit(int heal)
    {
        currentUnitHealth += heal;
        if (currentUnitHealth > maxHp + playerMaxHp)
        {
            currentUnitHealth = maxHp;
        }
    }
    public StatusEffect GetUnitStatusEffect()
    {
        return unitStatusEffect;
    }
    public void RemoveUnitStatusEffect()
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
                statusEffectString = $"{unitData.DisplayName} suffered from burning!";
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
                    statusEffectString = $"{unitData.DisplayName} suffered from poison!";
                else
                    statusEffectString = $"{unitData.DisplayName} suffered from poison badly!";
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
                statusEffectString = $"{unitData.DisplayName} woke up!";
                return false;
            }
            if (unitStatusEffect == StatusEffect.SLEEP)
                statusEffectString = $"{unitData.DisplayName} is still asleep!";
            else if (unitStatusEffect == StatusEffect.FROZEN)
                statusEffectString = $"{unitData.DisplayName} is still frozen!";
            turnDownCount += 1;
            Debug.Log($"{unitStatusEffect} TURN COUNT: {turnDownCount}");
            return true;
        }
        else if (unitStatusEffect == StatusEffect.PARALYSIS)
        {
            if (RandomRoller(100, paralysisPassTurnChance))
            {
                statusEffectString = $"{unitData.DisplayName} is unable to move!";
                return true;
            }
        }
        else if ((unitStatusEffect == StatusEffect.SLEEP && turnDownCount >= sleepMaxTurnCount) || (unitStatusEffect == StatusEffect.FROZEN && turnDownCount >= frozenMaxTurnCount))
        {
            unitStatusEffect = StatusEffect.NONE;
            if (unitStatusEffect == StatusEffect.SLEEP)
                statusEffectString = $"{unitData.DisplayName} woke up!";
            else if (unitStatusEffect == StatusEffect.FROZEN)
                statusEffectString = $"{unitData.DisplayName} thawed out!";
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
        return unitTakingDamage.TakeSpecialDamage(moveElementType, specialAttack, level, power, CalculateDamageBurnReduction());
    }
    public bool TakePhysicalDamage(ElementTypes moveElementType, int attackerAttack, int attackerLevel, int movePower, float burnMultiplier)
    {
        float type1Reaction = CalculateElementEffectiveMultiplier(moveElementType, unitData.Type1);
        moveEffectivenessType1 = SetTypeEffectiveness(type1Reaction);
        float calculatedDamage = (((2 * attackerLevel / 5) + 2) * movePower * (attackerAttack / defence) / 50 + 2) * type1Reaction * CalculateCriticalMultiplier() * burnMultiplier;
        if (unitData.Type2 != ElementTypes.NONE)
        {
            float type2Reaction = CalculateElementEffectiveMultiplier(moveElementType, unitData.Type2);
            calculatedDamage *= type2Reaction;
            moveEffectivenessType2 = SetTypeEffectiveness(type2Reaction);
        }
        Debug.Log($"Damage Calculated {calculatedDamage}");
        Debug.Log($"Effectiveness for defender has been calculated {moveEffectivenessType1} || {moveEffectivenessType2}");
        if (calculatedDamage > 0)
        {
            currentUnitHealth -= Mathf.FloorToInt (calculatedDamage);
            //Debug.Log($"{attackerName} has dealt {calculatedDamage} to {unitData.name}");
        }
        else
        {
            Debug.Log("Minimum amount of damage has been applied.");
            currentUnitHealth -= 1;
        }

        if (currentUnitHealth <= 0)
        {
            currentUnitHealth = 0;
            return true;
        }
        else
            return false;
    }
    public bool TakeSpecialDamage(ElementTypes moveElementType, int specialAttack, int attackerLevel, int movePower, float burnMultiplier)
    {
        float type1Reaction = CalculateElementEffectiveMultiplier(moveElementType, unitData.Type1);
        float calculatedDamage = (((2 * attackerLevel / 5) + 2) * movePower * (specialAttack / specialDefence) / 50 + 2) * type1Reaction * CalculateCriticalMultiplier() * burnMultiplier;
        if (unitData.Type2 != ElementTypes.NONE)
        {
            float type2Reaction = CalculateElementEffectiveMultiplier(moveElementType, unitData.Type2);
            calculatedDamage *= type2Reaction;
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
    #region Calculations for RNG, Damage multiplier (Burn, Crit, super effective)
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
    private ElementEffectiveness SetTypeEffectiveness(float effectivenessMultiplier)
    {
        if (effectivenessMultiplier == superEffectiveMultiplier)
            return ElementEffectiveness.SUPEREFFECTIVE;
        else if (effectivenessMultiplier == notSuperEffectiveMultiplier)
            return ElementEffectiveness.NOTEFFECTIVE;
        return ElementEffectiveness.NONE;
    }
    public float CalculateCriticalMultiplier()
    {
        if (RandomRoller(maxCriticalChance, criticalRate))
        {
            Debug.Log("CRIT!");
            isDefenderTakeCriticalHit = true;
            return criticalMultiplier;
        }
        Debug.Log("NOT CRIT!");
        isDefenderTakeCriticalHit = false;
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
    #endregion

    public bool IsUnitDropReward()
    {
        return RandomRoller(100,50);
    }
    public MovesData GetRandomMove()
    {
        int randomNum = Random.Range(0, unitData.DropMoveList.Count);
        Debug.Log(randomNum);
        return unitData.DropMoveList[randomNum];
    }
    public ElementEffectiveness GetMoveEffectiveness()
    {
        //if the unit is mono type just return the type 1 effectiveness.
        if (unitData.Type2 == ElementTypes.NONE) { return moveEffectivenessType1; }

        if (moveEffectivenessType1 == ElementEffectiveness.SUPEREFFECTIVE && moveEffectivenessType2 == ElementEffectiveness.SUPEREFFECTIVE)
            return ElementEffectiveness.SUPEREFFECTIVE;
        else if (moveEffectivenessType1 == ElementEffectiveness.NONE)
            return moveEffectivenessType2;
        else if (moveEffectivenessType2 == ElementEffectiveness.NONE)
            return moveEffectivenessType1;
        else if (moveEffectivenessType1 == ElementEffectiveness.NOTEFFECTIVE && moveEffectivenessType2 == ElementEffectiveness.NOTEFFECTIVE)
            return ElementEffectiveness.NOTEFFECTIVE;
        else
            return ElementEffectiveness.NONE;
    }
}

public enum ElementEffectiveness
{
    NONE,
    SUPEREFFECTIVE,
    NOTEFFECTIVE
};
