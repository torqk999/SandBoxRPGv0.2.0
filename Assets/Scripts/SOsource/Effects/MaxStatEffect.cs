using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public RawStatPackage StatAdjustPack;

    public override void ApplySingleEffect(Character target,  bool cast = false)
    {
        base.ApplySingleEffect(target, cast); // Risidual proc
    }
    public override void Amplify(float amp)
    {
        StatAdjustPack.Amplify(amp);
    }
    public override void InitializeRoot(GameState state)
    {
        StatAdjustPack.Initialize();
    }
    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)options.Source;

        StatAdjustPack.Clone(maxSource.StatAdjustPack);
    }
    /*public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "MaxStatEffect";
        return (MaxStatEffect)base.GenerateRootObject(options);
    }
    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }*/
}
