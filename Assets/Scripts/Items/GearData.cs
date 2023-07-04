using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearData : MonoBehaviour
{
    private List<UnitStats> headGearMainStatList = new() { UnitStats.HEALTH };
    private List<UnitStats> chestGearMainStatList = new() { UnitStats.DEFENCE, UnitStats.SPECIALDEFENCE };
    private List<UnitStats> legGearMainStatList = new() { UnitStats.SPEED };
    private List<UnitStats> allGearStatList = new() { UnitStats.HEALTH, UnitStats.DEFENCE, UnitStats.SPECIALDEFENCE, UnitStats.SPEED, UnitStats.ATTACK, UnitStats.SPECIALATTACK, UnitStats.SPEED };

    private int maxPositiveStatRoll = 11;
    private int maxNegativeStatRoll = 11;

    public UnitStats GetHeadMainStat()
    {
        return headGearMainStatList[RandomNumber(headGearMainStatList.Count)];
    }
    public UnitStats GetChestMainStat()
    {
        return chestGearMainStatList[RandomNumber(chestGearMainStatList.Count)];
    }
    public UnitStats GetLegMainStat()
    {
        return legGearMainStatList[RandomNumber(legGearMainStatList.Count)];
    }
    public UnitStats GetRandomStat()
    {
        return allGearStatList[RandomNumber(allGearStatList.Count)];
    }
    public int GetRandomPositiveStatNum()
    {
        return Random.Range(1, maxPositiveStatRoll);
    }
    public int GetRandomNegativeStatNum()
    {
        return -Random.Range(1, maxNegativeStatRoll);
    }
    private int RandomNumber(int max)
    {
        int num = Random.Range(0, max);
        return num;
    }
}
