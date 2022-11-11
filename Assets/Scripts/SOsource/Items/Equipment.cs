using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ItemObject
{
    [Header("Equipment Properties")]
    public SkillType EquipSkill;
    public ClassType ClassType;
    public int EquipLevel;
    public int EquipID;
    public CharacterAbility[] Abilities;
    public BaseEffect[] Effects;

    public virtual Equipment GenerateCloneEquip(int equipId = -1, bool inject = false, string instanceType = "Equipment")
    {
        //Debug.Log($"InstanceType: {instanceType}");
        Equipment newEquip = (Equipment)CloneItem(instanceType);

        newEquip.EquipSkill = EquipSkill;
        newEquip.ClassType = ClassType;
        newEquip.EquipLevel = EquipLevel;
        newEquip.EquipID = equipId;

        newEquip.CloneAbilitiesAndEffects(this, 1, inject);

        return newEquip;
    }
    public void CloneAbilitiesAndEffects(Equipment source, float amp = 1, bool inject = false)
    {
        Abilities = new CharacterAbility[source.Abilities.Length];
        for (int i = 0; i < Abilities.Length; i++)
        {
            Abilities[i] = source.Abilities[i].GenerateAbility();
        }

        Effects = new BaseEffect[source.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = source.Effects[i].GenerateEffect();
        }
    }
}
