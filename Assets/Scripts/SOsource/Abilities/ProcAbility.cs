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
    public override CharacterAbility GenerateAbility(RootOptions options)
    {
        //options.Root = "ProcAbility";
        ProcAbility newAbility = (ProcAbility)GenerateRootObject(options);
        newAbility.Copy(this, options);
        Debug.Log("Proc Ability Generated!");
        return newAbility;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        Debug.Log("wtf...");
        base.Copy(source, options);
        Debug.Log("wtf...");
        if (!(source is ProcAbility))
            return;
        Debug.Log("wtf...");
        ProcAbility procSource = (ProcAbility)source;
        Debug.Log("wtf...");
        Projectile = procSource.Projectile;

        Debug.Log("Proc Ability Copied!");
    }

}
