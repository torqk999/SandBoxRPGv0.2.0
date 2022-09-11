using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    public Ring CloneRing()
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
        newRing.EquipAbilites = new CharacterAbility[EquipAbilites.Length];
        for (int i = 0; i < EquipAbilites.Length; i++)
            newRing.EquipAbilites[i] = EquipAbilites[i];

        return newRing;
    }
}
