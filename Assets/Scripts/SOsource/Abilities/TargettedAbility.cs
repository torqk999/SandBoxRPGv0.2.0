using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargettedAbility : CharacterAbility
{
    [Header("Target Properties")]
    public BaseEffect[] Effects;
    public TargetType AbilityTarget;
    public float AOE_Range;
    public override void UseAbility(Character target)
    {
        for (int i = 0; i < Effects.Length; i++)
            Effects[i].ApplySingleEffect(target, true, EquipID); // First or only proc
    }

    public virtual void CloneEffects(TargettedAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        Effects = new BaseEffect[source.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
            Effects[i] = source.Effects[i].GenerateEffect(potency, inject, equipId);
    }

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        if (!(source is TargettedAbility))
            return;

        TargettedAbility targetSource = (TargettedAbility)source;

        AbilityTarget = targetSource.AbilityTarget;
        AOE_Range = targetSource.AOE_Range;

        CloneEffects(targetSource, equipId, potency, inject);
        base.CloneAbility(source, equipId);
    }
}
