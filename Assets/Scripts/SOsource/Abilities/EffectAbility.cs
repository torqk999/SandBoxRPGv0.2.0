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

    public virtual void CloneEffects(EffectAbility source, bool inject = false)
    {
        Effects = new BaseEffect[source.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
            if (source.Effects[i] != null)
                Effects[i] = source.Effects[i].GenerateEffect(null, inject);
    }
    public override void CloneAbility(CharacterAbility source, bool inject = false)
    {
        if (!(source is EffectAbility))
            return;

        EffectAbility targetSource = (EffectAbility)source;

        AbilityTarget = targetSource.AbilityTarget;

        CloneEffects(targetSource, inject);
        base.CloneAbility(source, inject);
    }
}
