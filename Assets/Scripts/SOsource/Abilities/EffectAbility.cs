using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectAbility : CharacterAbility
{
    [Header("Target Properties")]
    public BaseEffect[] Effects;
    public TargetType AbilityTarget;

    public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        Debug.Log($"Using ability: {Name}");
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i].ApplySingleEffect(target, options); // First or only proc
            if (i == 0)
                options.IsProjectile = false; // make sure to set projectile to false
        }

        base.UseAbility(target, options);
    }
    public override void Amplify(CharacterSheet sheet = null, Equipment equip = null)
    {
        StringBuilder debug = new StringBuilder();

        Debug.Log($"Comps: {sheet != null}:{equip != null}");
        float amp = CharacterMath.GeneratePotency(ref debug, sheet, equip);
        Debug.Log($"Potency: {debug}");
        foreach (BaseEffect effect in Effects)
            effect.Amplify(amp);
    }
    public override void InitializeSource()
    {
        foreach (BaseEffect effect in Effects)
            effect.InitializeSource();
    }

    public virtual void ProduceOriginalEffects(EffectAbility source, EffectOptions options = default(EffectOptions))
    {
        Effects = new BaseEffect[source.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
            if (source.Effects[i] != null)
            {
                Effects[i] = source.Effects[i].GenerateEffect(options);
                Effects[i].Logic.ProjectileLength = ProjectileLength;
            }    
    }
    public override void CloneAbility(CharacterAbility source)
    {
        if (!(source is EffectAbility))
            return;

        EffectAbility targetSource = (EffectAbility)source;

        AbilityTarget = targetSource.AbilityTarget;

        ProduceOriginalEffects(targetSource);
        base.CloneAbility(source);
    }
}
