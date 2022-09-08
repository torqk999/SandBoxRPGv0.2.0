using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*public enum EquipType
{
    WEARABLE,
    ONEHAND,
    OFFHAND,
    TWOHAND
}*/

public class Equipment : ItemObject
{
    [Header("Equipment Properties")]
    public SkillType EquipSkill;
    public int EquipLevel;
    public int AbilityID;
    public CharacterAbility[] EquipAbilites;

    public Equipment CloneEquip()
    {
        Equipment newEquip = (Equipment)ScriptableObject.CreateInstance("Equipment");

        newEquip.itemID = itemID;
        newEquip.Name = Name;
        newEquip.Sprite = Sprite;
        newEquip.Quality = Quality;
        newEquip.GoldValue = GoldValue;
        newEquip.Weight = Weight;

        newEquip.EquipSkill = EquipSkill;
        newEquip.EquipLevel = EquipLevel;
        newEquip.AbilityID = AbilityID;
        newEquip.EquipAbilites = new CharacterAbility[EquipAbilites.Length];
        for (int i = 0; i < EquipAbilites.Length; i++)
            newEquip.EquipAbilites[i] = EquipAbilites[i];

        return newEquip;
    }
}
