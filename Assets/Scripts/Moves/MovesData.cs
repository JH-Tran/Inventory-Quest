using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Move Data")]
public class MovesData : ScriptableObject
{
    public string moveName;
    [TextArea]
    public string description;
    public MoveType moveType;

    //Basic Move Stats
    public int power;
    public int accuracy;
    public ElementTypes elementalType;
    public StatusEffect statusEffect;
    public int statusAccuracy;
    public UnitStats unitStats;
}

public enum MoveType { 
    NONE,
    PHYSICAL,
    SPECIAL,
    BUFF,
    DEBUFF,
    STATUS,
};
