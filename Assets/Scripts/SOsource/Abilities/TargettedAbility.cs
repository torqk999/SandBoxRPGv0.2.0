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

    public BaseEffect CreateEffectInstance(BaseEffect source)
    {
        switch (source)
        {
            default:
                return (BaseEffect)CreateInstance("BaseEffect");
        }
    }

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source, equipId);

        if (!(source is TargettedAbility))
            return;

        TargettedAbility targetSource = (TargettedAbility)source;

        AbilityTarget = targetSource.AbilityTarget;
        AOE_Range = targetSource.AOE_Range;
    }
}
