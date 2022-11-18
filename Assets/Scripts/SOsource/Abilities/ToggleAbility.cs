using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ToggleAbility", menuName = "ScriptableObjects/Abilities/Toggle")]
public class ToggleAbility : EffectAbility
{
    [Header("Toggle Properties")]
    public ParticleSystem Aura;
    public bool Active;

    public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        Active = !Active;

        options.EffectType = EffectType.TOGGLE;
        options.ToggleActive = Active;
        options.IsProjectile = true;
        //options.IsClone = true;
        //options.Inject = false;

        base.UseAbility(target, options);
    }

    public override void ProduceOriginalEffects(EffectAbility source, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.TOGGLE;
        options.ToggleActive = false;
        options.IsProjectile = false;
        //options.IsClone = false;
        //options.Inject = false;

        base.ProduceOriginalEffects(source, options);
    }

    public override void Clone(RootScriptObject source, RootOptions options = default)
    {
        base.Clone(source, options);

        if (!(source is ToggleAbility))
            return;

        ToggleAbility passiveSource = (ToggleAbility)source;

        Aura = passiveSource.Aura;
        Active = false;
    }
    public override CharacterAbility GenerateAbility(RootOptions options)
    {
        options.ClassID = "ToggleAbility";
        ToggleAbility newAbility = (ToggleAbility)GenerateRootObject(options);
        newAbility.Clone(this, options);
        return newAbility;
    }
}
