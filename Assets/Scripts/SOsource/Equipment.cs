using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ItemObject
{
    [Header("Equipment Properties")]
    public SkillType EquipSkill;
    public int EquipLevel;
    public int EquipID;
    public CharacterAbility[] EquipAbilities;

    public Equipment CloneEquip(string instanceType = "Equipment", int equipId = -1, bool inject = false)
    {
        Equipment newEquip = (Equipment)CloneItem(instanceType);

        newEquip.EquipSkill = EquipSkill;
        newEquip.EquipLevel = EquipLevel;
        newEquip.EquipID = equipId;

        newEquip.CloneAbilities(EquipAbilities, 1, inject);

        return newEquip;
    }
    public void CloneAbilities(CharacterAbility[] source, float amp = 1, bool inject = false)
    {
        EquipAbilities = new CharacterAbility[source.Length];
        for (int i = 0; i < EquipAbilities.Length; i++)
        {
            EquipAbilities[i] = (CharacterAbility)ScriptableObject.CreateInstance("CharacterAbility");
            EquipAbilities[i].Clone(source[i], EquipID, amp, inject);
        }
    }
}
