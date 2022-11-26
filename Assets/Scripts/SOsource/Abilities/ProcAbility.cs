using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProcAbility", menuName = "ScriptableObjects/Abilities/Proc")]
public class ProcAbility : EffectAbility
{
    [Header("Proc Properties")]
    public ParticleSystem Projectile;

    /*public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.PROC;
        options.ToggleActive = true;
        options.IsProjectile = true;
        //options.IsClone = true;
        //options.Inject = false;

        base.UseAbility(target, options);
    }
    public override void ProduceOriginalEffects(EffectAbility source, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.PROC;
        options.ToggleActive = true;
        options.IsProjectile = true;
        //options.IsClone = false;
        //options.Inject = false;

        base.ProduceOriginalEffects(source, options);
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        ProcAbility newAbility = (ProcAbility)GenerateRootObject(options);
        newAbility.Clone(options);
        Debug.Log("Proc Ability Generated!");
        return newAbility;
    }*/
    public override void Clone(RootOptions options)
    {
        Debug.Log("Copying proc ability...");
        base.Clone(options);
        if (!(options.Source is ProcAbility))
            return;

        ProcAbility procSource = (ProcAbility)options.Source;
        Projectile = procSource.Projectile;

        Debug.Log("Proc Ability Copied!");
    }

}
