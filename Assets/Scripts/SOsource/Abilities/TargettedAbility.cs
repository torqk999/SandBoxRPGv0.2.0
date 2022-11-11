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

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source, equipId);

        if (!(source is TargettedAbility))
            return;

        TargettedAbility targetSource = (TargettedAbility)source;

        AbilityTarget = targetSource.AbilityTarget;
        AOE_Range = targetSource.AOE_Range;

        Effects = new BaseEffect[targetSource.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
            Effects[i] = targetSource.Effects[i].GenerateEffect(equipId, potency, inject);
    }
}
