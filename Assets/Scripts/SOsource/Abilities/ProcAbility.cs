using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : EffectAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Projectile;

    public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.PROC;
        options.ToggleActive = true;
        options.IsProjectile = true;
        options.IsClone = true;
        options.Inject = false;

        base.UseAbility(target, options);
    }
    public override void ProduceOriginalEffects(EffectAbility source, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.PROC;
        options.ToggleActive = true;
        options.IsProjectile = true;
        options.IsClone = false;
        options.Inject = false;

        base.ProduceOriginalEffects(source, options);
    }

    public override void CloneAbility(CharacterAbility source)
    {
        if (!(source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)source;

        Projectile = procSource.Projectile;

        base.CloneAbility(source);
    }

    public override CharacterAbility GenerateAbility()
    {
        ProcAbility newAbility = (ProcAbility)CreateInstance("ProcAbility");
        newAbility.CloneAbility(this);
        return newAbility;
    }
}
