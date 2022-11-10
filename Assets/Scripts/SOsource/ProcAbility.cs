using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static CharacterRender;

public enum RangeType
{
    SELF,
    TARGET,
    TARGET_AOE,
    SELF_AOE
}
public enum TargetType
{
    ALL,
    ALLY,
    ENEMY
}
public enum AbilitySchool
{
    ATTACK,
    SPELL,
    POWER
}

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : CharacterAbility
{
    [Header("Proc Properties")]
    public TargetType Target;
    public RangeType RangeType;
    
    public Effect[] Effects;

    public void CloneProc(ProcAbility ability, int equipId = -1, float potency = 1, bool inject = false)
    {
        CloneAbility(ability, equipId);

        Target = ability.Target;
        RangeType = ability.RangeType;

        Effects = new Effect[ability.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = (Effect)CreateInstance("Effect");
            Effects[i].CloneEffect(ability.Effects[i], equipId, potency, inject);
        }
    }
    override public CharacterAbility EquipAbility(Character currentCharacter, Equipment equip, bool inject)
    {
        ProcAbility newAbility = (ProcAbility)CreateInstance("ProcAbility");
        newAbility.CloneProc(this, equip.EquipID, GeneratePotency(currentCharacter, equip), inject);
        return newAbility;
    }
}
