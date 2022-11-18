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
    public override void InitializeRoot(GameState state)
    {
        StatAdjustPack.Initialize();
    }
    public override void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        base.CloneEffect(source, effectOptions, effected);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        StatAdjustPack.Clone(maxSource.StatAdjustPack);
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = "MaxStatEffect";
        return (MaxStatEffect)base.GenerateRootObject(options);
    }
    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }
}
