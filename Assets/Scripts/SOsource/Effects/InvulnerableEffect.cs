using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invulnerability", menuName = "ScriptableObjects/Effects/Invulnerability")]
public class InvulnerableEffect : BaseEffect
{
    [Header("Invulnerable Properties")]
    public RawStat TargetStat;

    public override void CloneEffect(BaseEffect source, EffectOptions options)
    {
        base.CloneEffect(source, options);

        if (!(source is InvulnerableEffect))
            return;

        InvulnerableEffect invulnerableSource = (InvulnerableEffect)source;

        TargetStat = invulnerableSource.TargetStat;
    }

    public override BaseEffect GenerateEffect(EffectOptions options, Character effected = null)
    {
        InvulnerableEffect newEffect = (InvulnerableEffect)CreateInstance("InvulnerableEffect");
        newEffect.CloneEffect(this, options);
        return newEffect;
    }
}
