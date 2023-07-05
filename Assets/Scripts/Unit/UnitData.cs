using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit Data")]
public class UnitData : ScriptableObject
{
    //Unit label
    [SerializeField] string id;
    [SerializeField] string displayName;
    [SerializeField] Sprite unitSprite;
    [SerializeField] Color unitColour = Color.white;
    [TextArea]
    public string description;
    //Unit starting combat values
    [SerializeField] int startingLevel = 1;
    [SerializeField] int maxHealthPoint;
    [SerializeField] int attack;
    [SerializeField] int specialAttack;
    [SerializeField] int defence;
    [SerializeField] int specialDefence;
    [SerializeField] int speed;
    //Unit elementl type
    [SerializeField] ElementTypes type1;
    [SerializeField] ElementTypes type2;
    //Status effects
    [SerializeField] bool isConfused;
    [SerializeField] bool isBound;
    //Moves
    [SerializeField] List<MovesData> dropMoveList = new(4);
    //Experience
    [SerializeField] int baseExperience;

    public int MaxHealthPoint => maxHealthPoint;
    public int Attack => attack;
    public int SpecialAttack => specialAttack;
    public int Defence => defence;
    public int SpecialDefence => specialDefence;
    public int Speed => speed;
    public string DisplayName => displayName;
    public int StartingLevel => startingLevel;
    public List<MovesData> DropMoveList => dropMoveList;
    public ElementTypes Type1 => type1;
    public ElementTypes Type2 => type2;
    public Sprite UnitSprite => unitSprite;
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
