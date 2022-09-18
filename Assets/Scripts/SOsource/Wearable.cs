using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WearableType
{
    HEAD = 0,
    CHEST = 1,
    AMULET = 2,
    SHOULDERS = 3,
    BELT = 4,
    LEGS = 5,
    FEET = 6,
    RING = 7,
    GLOVES = 8,
}

[CreateAssetMenu(fileName = "Wearable", menuName = "ScriptableObjects/Equipment/Wearable")]
public class Wearable : Equipment
{
    [Header("Wearable Properties")]
    public EquipSlot Type;
    //public EquipSkill EquipSkill;

    public Wearable CloneWear(int equipId = -1, bool inject = false)
    {
        Wearable newWear = (Wearable)CloneEquip("Wearable", equipId, inject);

        newWear.Type = Type;
        
        return newWear;
    }
}