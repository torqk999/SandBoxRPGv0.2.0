using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectAbility : CharacterAbility
{
    [Header("Target Properties")]
    public List<RootScriptObject> Effects;
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
    public override void UseAbility(Character target)
    {
        Debug.Log($"Using ability: {Name}");
        for (int i = 0; i < Effects.Count; i++)
        {
            ((BaseEffect)Effects[i]).ApplySingleEffect(target, true);
        }

        base.UseAbility(target);
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
    public virtual void ProduceOriginalEffects(EffectAbility source)
    {
        Debug.Log("Producing original effects...");

        RootOptions rootOptions = new RootOptions(source.RootLogic.Options.GameState, null, ref source.RootLogic.Options.GameState.EFFECT_INDEX, source.Effects);
        EffectOptions effectOptions = new EffectOptions(source);
        Effects = new List<RootScriptObject>(source.Effects.Count);
        for (int i = 0; i < Effects.Count; i++)
            if (source.Effects[i] != null)
            {
                rootOptions.Source = source.Effects[i];
                rootOptions.Index = i;
                Debug.Log($"{i} oh shit...");
                Effects[i] = ((BaseEffect)source.Effects[i]).GenerateEffect(rootOptions, i == 0); // first effect will be the projectile
                ((BaseEffect)Effects[i]).InitializeEffect(rootOptions.GameState, effectOptions);
                Debug.Log($"{i} oh shit...");
            }
        Debug.Log("effects generated on effect ability!");
    }

    public override void InitializeRoot(GameState state)
    {
        Debug.Log("Initializing EffectAbility...");
        base.InitializeRoot(state);
        EffectOptions options = new EffectOptions(this);

        foreach (BaseEffect effect in Effects)
            effect.InitializeEffect(state, options);
    }
    /*public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = options.Root == "" ? "EffectAbility" : options.Root;
        EffectAbility newAbility = (EffectAbility)GenerateRootObject(options);
        newAbility.Clone(options);
        return newAbility;
    }*/
    public override void Clone(RootOptions options)
    {
        Debug.Log("Copying effect ability...");
        base.Clone(options);

        if (!(options.Source is EffectAbility))
            return;

        EffectAbility targetSource = (EffectAbility)options.Source;
        AbilityTarget = targetSource.AbilityTarget;

        ProduceOriginalEffects(targetSource);
        Debug.Log("Effect ability copied!");
    }
    
}
