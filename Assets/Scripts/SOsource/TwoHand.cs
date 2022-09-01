using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwoHandType
{
    AXE,
    CLAYMORE,
    POLEARM,
    BARDICHE,
    SPEAR,
    STAFF,
    BOW,
    CROSSBOW
}

[CreateAssetMenu(fileName = "TwoHand", menuName = "ScriptableObjects/Equipment/TwoHand")]
public class TwoHand : Equipment
{
    [Header("TwoHand Properties")]
    public TwoHandType Type;

    public TwoHand CloneTwoHand()
    {
        TwoHand newTwoHand = (TwoHand)ScriptableObject.CreateInstance("TwoHand");

        newTwoHand.itemID = itemID;
        newTwoHand.Name = Name;
        newTwoHand.Sprite = Sprite;
        newTwoHand.Quality = Quality;
        newTwoHand.GoldValue = GoldValue;
        newTwoHand.Weight = Weight;

        newTwoHand.Type = Type;
        newTwoHand.EquipSkill = EquipSkill;
        newTwoHand.EquipLevel = EquipLevel;
        newTwoHand.AbilityID = AbilityID;
        newTwoHand.EquipAbilites = new CharacterAbility[EquipAbilites.Length];
        for (int i = 0; i < EquipAbilites.Length; i++)
            newTwoHand.EquipAbilites[i] = EquipAbilites[i];

        return newTwoHand;
    }
}