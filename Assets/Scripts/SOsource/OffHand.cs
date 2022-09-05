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

    public OffHand CloneOneHand()
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
        newOffHand.EquipAbilites = new CharacterAbility[EquipAbilites.Length];
        for (int i = 0; i < EquipAbilites.Length; i++)
            newOffHand.EquipAbilites[i] = EquipAbilites[i];

        return newOffHand;
    }

}
