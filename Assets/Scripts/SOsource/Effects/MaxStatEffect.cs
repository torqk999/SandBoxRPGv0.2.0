using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public StatPackage StatAdjustPack;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float amp = 1, bool inject = true)
    {
        base.CloneEffect(source);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        StatAdjustPack = new StatPackage(maxSource.StatAdjustPack);
        StatAdjustPack.Reflection.Reflect(ref StatAdjustPack.Stats, inject);
        StatAdjustPack.Amplify(amp);
    }
}
