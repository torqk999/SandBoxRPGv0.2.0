using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : BaseEffect
{
    [Header ("Stat Properties")]
    public ValueType Value;

    public override void CloneEffect(BaseEffect source,  bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is StatEffect))
            return;

        StatEffect statSource = (StatEffect)source;

        Value = statSource.Value;
    }
}
