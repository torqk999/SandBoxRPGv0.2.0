using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public RawStatPackage StatAdjustPack;

    public override void ApplySingleEffect(Character target, EffectOptions options, bool cast = false)
    {
        base.ApplySingleEffect(target, options, cast); // Risidual proc
    }
    public override void Amplify(float amp)
    {
        StatAdjustPack.Amplify(amp);
    }
    public override void InitializeSource()
    {
        StatAdjustPack.Initialize();
    }
    public override void CloneEffect(BaseEffect source, EffectOptions options)
    {
        base.CloneEffect(source, options);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        StatAdjustPack.Clone(maxSource.StatAdjustPack);
    }
    public override BaseEffect GenerateEffect(EffectOptions options, Character effected = null)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, options);
        return newEffect;
    }
}
