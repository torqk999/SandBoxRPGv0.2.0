using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : BaseEffect
{
    [Header ("Stat Properties")]
    public ValueType Value;

    public override void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        base.CloneEffect(source, effectOptions, effected);

        if (!(source is StatEffect))
            return;

        StatEffect statSource = (StatEffect)source;

        Value = statSource.Value;
    }
}
