using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : EffectAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Projectile;

    public override void UseAbility(Character target)
    {
        for (int i = 0; i < Effects.Length; i++)
            Effects[i].ApplySingleEffect(target, true, false); // First or only proc
    }
    public override void CloneEffects(EffectAbility source, bool inject = false)
    {
        base.CloneEffects(source, inject);
    }

    public override void CloneAbility(CharacterAbility source, bool inject = false)
    {
        if (!(source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)source;

        Projectile = procSource.Projectile;

        base.CloneAbility(source, inject);
    }

    public override CharacterAbility GenerateAbility(bool inject = false)
    {
        ProcAbility newAbility = (ProcAbility)CreateInstance("ProcAbility");
        newAbility.CloneAbility(this, inject);
        return newAbility;
    }
}
