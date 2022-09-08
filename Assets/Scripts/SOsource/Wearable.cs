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
    public WearableType Type;
    //public EquipSkill EquipSkill;

    public Wearable CloneWear()
    {
        Wearable newWear = (Wearable)ScriptableObject.CreateInstance("Wearable");

        newWear.itemID = itemID;
        newWear.Name = Name;
        newWear.Sprite = Sprite;
        newWear.Quality = Quality;
        newWear.GoldValue = GoldValue;
        newWear.Weight = Weight;

        newWear.Type = Type;
        newWear.EquipSkill = EquipSkill;
        newWear.EquipLevel = EquipLevel;
        newWear.AbilityID = AbilityID;
        newWear.EquipAbilites = new CharacterAbility[EquipAbilites.Length];
        for (int i = 0; i < EquipAbilites.Length; i++)
            newWear.EquipAbilites[i] = EquipAbilites[i];

        return newWear;
    }
}