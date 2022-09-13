using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    public Ring CloneRing(bool inject = false)
    {
        Ring newRing = (Ring)ScriptableObject.CreateInstance("OneHand");

        newRing.itemID = itemID;
        newRing.Name = Name;
        newRing.Sprite = Sprite;
        newRing.Quality = Quality;
        newRing.GoldValue = GoldValue;
        newRing.Weight = Weight;

        //newRing.Type = Type;
        newRing.EquipSkill = EquipSkill;
        newRing.EquipLevel = EquipLevel;
        newRing.AbilityID = AbilityID;

        newRing.CloneAbilities(EquipAbilities, 1, inject);

        return newRing;
    }
}
