using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : BaseEffect
{
    [Header ("Stat Properties")]
    public ValueType Value;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId);

        if (!(source is StatEffect))
            return;

        StatEffect statSource = (StatEffect)source;

        Value = statSource.Value;
    }
}
