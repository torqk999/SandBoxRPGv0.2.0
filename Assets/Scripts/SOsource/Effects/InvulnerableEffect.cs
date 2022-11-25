using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invulnerability", menuName = "ScriptableObjects/Effects/Invulnerability")]
public class InvulnerableEffect : BaseEffect
{
    [Header("Invulnerable Properties")]
    public RawStat TargetStat;

    public override void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        base.CloneEffect(source, effectOptions, effected);

        if (!(source is InvulnerableEffect))
            return;

        InvulnerableEffect invulnerableSource = (InvulnerableEffect)source;

        TargetStat = invulnerableSource.TargetStat;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "InvulnerableEffect";
        return (InvulnerableEffect)base.GenerateRootObject(options);
    }
    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        InvulnerableEffect newEffect = (InvulnerableEffect)CreateInstance("InvulnerableEffect");
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }
}
