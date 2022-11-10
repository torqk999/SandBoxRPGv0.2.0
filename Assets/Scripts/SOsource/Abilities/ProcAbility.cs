using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : TargettedAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Projectile;

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source);

        if (!(source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)source;
        
        Effects = new BaseEffect[procSource.Effects.Length];

        for (int i = 0; i < Effects.Length; i++)
            Effects[i] = procSource.Effects[i].GenerateEffect(equipId, potency, inject);
    }

    public override CharacterAbility GenerateAbility(Character currentCharacter, bool inject, Equipment equip = null)
    {
        ProcAbility newAbility = (ProcAbility)CreateInstance("ProcAbility");
        int id = equip == null ? -1 : equip.EquipID;
        newAbility.CloneAbility(this, id, currentCharacter.GeneratePotency(equip), inject);
        return newAbility;
    }
}
