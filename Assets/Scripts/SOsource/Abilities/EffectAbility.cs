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

    public override bool HasEligableTarget()
    {
        if (!base.HasEligableTarget())
            return false;

        foreach (BaseEffect effect in Effects)
            if (effect.HasEligableTarget())
                return true;

        return false;
    }
    public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        Debug.Log($"Using ability: {Name}");
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i].ApplySingleEffect(target, options, true); // First or only proc
            if (options.IsProjectile &&
                Logic.CastTargetInstance == null)
            {

            }
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
    public virtual void ProduceOriginalEffects(EffectAbility source, EffectOptions effectOptions = default(EffectOptions))
    {
        RootOptions rootOptions = new RootOptions(ref GameState.ROOT_SO_INDEX);
        Effects = new BaseEffect[source.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
            if (source.Effects[i] != null)
            {
                Effects[i] = source.Effects[i].GenerateEffect(rootOptions, effectOptions);
                Effects[i].ProjectileDuration = source.Effects[i].ProjectileDuration;
            }    
    }

    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        foreach (BaseEffect effect in Effects)
            effect.InitializeRoot(state);
    }
    public override CharacterAbility GenerateAbility(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "EffectAbility" : options.ClassID;
        EffectAbility newAbility = (EffectAbility)GenerateRootObject(options);
        newAbility.Copy(this, options);
        return newAbility;
    }
    public override void Copy(RootScriptObject source, RootOptions options = default)
    {
        base.Copy(source, options);

        if (!(source is EffectAbility))
            return;

        EffectAbility targetSource = (EffectAbility)source;
        AbilityTarget = targetSource.AbilityTarget;

        ProduceOriginalEffects(targetSource);
    }
    
}
