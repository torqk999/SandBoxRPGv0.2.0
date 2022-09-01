using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OneHandType
{
    DAGGER,
    WAND,
    RAPIER,
    AXE,
    MACE,
    FLAIL,
    SHIELD,
    BUCKLER
}

[CreateAssetMenu(fileName = "OneHand", menuName = "ScriptableObjects/Equipment/OneHand")]
public class OneHand : Equipment
{
    [Header("OneHand Properties")]
    public OneHandType Type;

    public OneHand CloneOneHand()
    {
        OneHand newOneHand = (OneHand)ScriptableObject.CreateInstance("OneHand");

        newOneHand.itemID = itemID;
        newOneHand.Name = Name;
        newOneHand.Sprite = Sprite;
        newOneHand.Quality = Quality;
        newOneHand.GoldValue = GoldValue;
        newOneHand.Weight = Weight;

        newOneHand.Type = Type;
        newOneHand.EquipSkill = EquipSkill;
        newOneHand.EquipLevel = EquipLevel;
        newOneHand.AbilityID = AbilityID;
        newOneHand.EquipAbilites = new CharacterAbility[EquipAbilites.Length];
        for (int i = 0; i < EquipAbilites.Length; i++)
            newOneHand.EquipAbilites[i] = EquipAbilites[i];

        return newOneHand;
    }
}