using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public RawStatPackage StatAdjustPack;

    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        StatAdjustPack.Clone(maxSource.StatAdjustPack);
        StatAdjustPack.Reflect(inject);
    }

    public override BaseEffect GenerateEffect(bool inject = true)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
}
