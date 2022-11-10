using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : TargettedAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Cast;
    public ParticleSystem Projectile;

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source);

        if (!(source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)source;
        
        Effects = new BaseEffect[procSource.Effects.Length];

        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = CreateEffectInstance(procSource.Effects[i]);//(BaseEffect)CreateInstance("BaseEffect");
            Effects[i].CloneEffect(procSource.Effects[i], -1, potency, inject);
        }
    }
    public override CharacterAbility EquipAbility(Character currentCharacter, Equipment equip, bool inject)
    {
        ProcAbility newProcAbility = (ProcAbility)CreateInstance("ProcAbility");
        newProcAbility.CloneAbility(this, equip.EquipID, GeneratePotency(currentCharacter, equip), inject);
        return newProcAbility;
    }
}
