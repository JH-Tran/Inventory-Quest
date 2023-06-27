using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit Data")]
public class UnitData : ScriptableObject
{
    //Unit label
    public string id;
    public string displayName;
    public Sprite unitSprite;
    public Color unitColour = Color.white;
    [TextArea]
    public string description;
    //Unit starting combat values
    public int startingLevel = 1;
    public int maxHealthPoint;
    public int attack;
    public int specialAttack;
    public int defence;
    public int specialDefence;
    public int speed;
    //Unit elementl type
    public ElementTypes type1;
    public ElementTypes type2;
    //Status effects
    public bool isConfused;
    public bool isBound;
    //Moves
    public List<MovesData> dropMoveList = new(4);
}

//Status effect that can only be one at a time
public enum StatusEffect
{
    NONE,
    SLEEP,
    PARALYSIS,
    BURN,
    POISON,
    FROZEN,
};
//Elemental Types
public enum ElementTypes
{
    NONE,
    NORMAL,
    FIRE,
    WATER,
    GRASS,
    LIGHT,
    DARK,
}
//Unit status that can be changed
public enum UnitStats
{
    HEALTH,
    ATTACK,
    SPECIALATTACK,
    DEFENCE,
    SPECIALDEFENCE, 
    SPEED,
}
