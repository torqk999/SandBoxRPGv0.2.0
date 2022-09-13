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
    public CharacterAbility[] EquipAbilities;

    public Equipment CloneEquip(bool inject = false)
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

        newEquip.CloneAbilities(EquipAbilities, 1, inject);

        return newEquip;
    }
    public void CloneAbilities(CharacterAbility[] source, float amp = 1, bool inject = false)
    {
        EquipAbilities = new CharacterAbility[source.Length];
        for (int i = 0; i < EquipAbilities.Length; i++)
        {
            EquipAbilities[i] = (CharacterAbility)ScriptableObject.CreateInstance("CharacterAbility");
            EquipAbilities[i].Clone(source[i], amp, inject);
        }
    }
}
