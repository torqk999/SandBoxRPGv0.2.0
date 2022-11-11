using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : TargettedAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Projectile;

    public override void CloneEffects(TargettedAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneEffects(source, -1, potency, inject);
    }

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        if (!(source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)source;

        Projectile = procSource.Projectile;
        equipId = -1; // Hard write procs

        base.CloneAbility(source, equipId, potency, inject);
    }

    public override CharacterAbility GenerateAbility(float potency = 1, bool inject = false, int equipId = 0)
    {
        ProcAbility newAbility = (ProcAbility)CreateInstance("ProcAbility");
        newAbility.CloneAbility(this, equipId, potency, inject);
        return newAbility;
    }
}
