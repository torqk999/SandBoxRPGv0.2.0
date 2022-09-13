using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OffHandType
{
    PARRY,
    SHIELD,
    BUCKLER
}

[CreateAssetMenu(fileName = "OffHand", menuName = "ScriptableObjects/Equipment/OffHand")]
public class OffHand : Equipment
{
    [Header("OffHand Properties")]
    public OffHandType Type;

    public OffHand CloneOneHand(bool inject = false)
    {
        OffHand newOffHand = (OffHand)ScriptableObject.CreateInstance("OffHand");

        newOffHand.itemID = itemID;
        newOffHand.Name = Name;
        newOffHand.Sprite = Sprite;
        newOffHand.Quality = Quality;
        newOffHand.GoldValue = GoldValue;
        newOffHand.Weight = Weight;

        newOffHand.Type = Type;
        newOffHand.EquipSkill = EquipSkill;
        newOffHand.EquipLevel = EquipLevel;
        newOffHand.AbilityID = AbilityID;

        newOffHand.CloneAbilities(EquipAbilities, 1, inject);

        return newOffHand;
    }

}
