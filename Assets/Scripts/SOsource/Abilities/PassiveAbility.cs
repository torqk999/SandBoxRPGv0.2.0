using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Abilities/Passive")]
public class PassiveAbility : EffectAbility
{
    [Header("Proc Properties")]
    public ParticleSystem AuraSelf;
    public ParticleSystem AuraAOE;
    //public BaseEffect[] Passives;
    public float ProcTimer;
    public float ProcDelay;

    /*public override void UseAbility(Character target)
    {
        options.EffectType = EffectType.PASSIVE;
        options.ToggleActive = true;
        options.IsProjectile = true;
        //options.IsClone = true;
        //options.Inject = false;

        base.UseAbility(target);
    }

    public override void ProduceOriginalEffects(EffectAbility source, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.PASSIVE;
        options.ToggleActive = true;
        options.IsProjectile = false;
        //options.IsClone = false;
        //options.Inject = false;

        base.ProduceOriginalEffects(source, options);
    }*/
    public override void UpdatePassiveTimer()
    {
        ProcTimer -= GlobalConstants.TIME_SCALE;
        if (ProcTimer <= 0)
        {
            ProcTimer = ProcDelay;
            PassiveProc();
        }
    }
    void PassiveProc()
    {
        if (Logic.SourceCharacter == null)
        {
            Debug.Log("No source!");
            return;
        }

        Logic.SourceCharacter.AttemptAbility(this);
    }

    /*public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "PassiveAbility";
        PassiveAbility newAbility = (PassiveAbility)GenerateRootObject(options);
        newAbility.Clone(options);
        return newAbility;
    }*/
    public override void Clone(RootOptions options = default)
    {
        base.Clone(options);

        if (!(options.Source is PassiveAbility))
            return;

        PassiveAbility passiveSource = (PassiveAbility)options.Source;

        AuraSelf = passiveSource.AuraSelf;
        AuraAOE = passiveSource.AuraAOE;

        //ClonePassives(passiveSource, inject);

        ProcDelay = passiveSource.ProcDelay;
        ProcTimer = ProcDelay;
    }
}
