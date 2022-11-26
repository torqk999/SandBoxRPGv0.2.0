using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invulnerability", menuName = "ScriptableObjects/Effects/Invulnerability")]
public class InvulnerableEffect : BaseEffect
{
    [Header("Invulnerable Properties")]
    public RawStat TargetStat;

    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is InvulnerableEffect))
            return;

        InvulnerableEffect invulnerableSource = (InvulnerableEffect)options.Source;

        TargetStat = invulnerableSource.TargetStat;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "InvulnerableEffect";
        return (InvulnerableEffect)base.GenerateRootObject(options);
    }
    /*public override BaseEffect GenerateEffect(RootOptions rootOptions, Character effected = null, bool projectile = false)
    {
        InvulnerableEffect newEffect = (InvulnerableEffect)CreateInstance("InvulnerableEffect");
        newEffect.CloneEffect(this, effected);
        return newEffect;
    }*/
}
