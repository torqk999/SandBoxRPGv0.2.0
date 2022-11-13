using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EffectAbility : CharacterAbility
{
    [Header("Target Properties")]
    public BaseEffect[] Effects;
    public TargetType AbilityTarget;
    
    public override void UseAbility(Character target)
    {
        //for (int i = 0; i < Effects.Length; i++)
        //    Effects[i].ApplySingleEffect(target, null, null, null, true); // First or only proc
    }
    public override void Amplify(CharacterSheet sheet = null, Equipment equip = null)
    {
        float amp = CharacterMath.GeneratePotency(sheet, equip);

    }
    public virtual void CloneEffects(EffectAbility source, bool inject = false)
    {
        Effects = new BaseEffect[source.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
            if (source.Effects[i] != null)
                Effects[i] = source.Effects[i].GenerateEffect(inject);
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
