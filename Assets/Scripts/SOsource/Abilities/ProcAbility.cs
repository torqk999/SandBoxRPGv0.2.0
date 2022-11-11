using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : TargettedAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Projectile;

    public override void CloneEffects(TargettedAbility source, Equipment equip = null, bool inject = false)
    {
        base.CloneEffects(source, equip, inject);
    }

    public override void CloneAbility(CharacterAbility source, Equipment equip = null, bool inject = false)
    {
        if (!(source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)source;

        Projectile = procSource.Projectile;

        base.CloneAbility(source, equip, inject);
    }

    public override CharacterAbility GenerateAbility(Equipment equip = null, bool inject = false)
    {
        ProcAbility newAbility = (ProcAbility)CreateInstance("ProcAbility");
        newAbility.CloneAbility(this, equip, inject);
        return newAbility;
    }
}
