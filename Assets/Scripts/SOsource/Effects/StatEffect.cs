using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : BaseEffect
{
    [Header ("Stat Properties")]
    public ValueType Value;

    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is StatEffect))
            return;

        StatEffect statSource = (StatEffect)options.Source;

        Value = statSource.Value;
    }
}
