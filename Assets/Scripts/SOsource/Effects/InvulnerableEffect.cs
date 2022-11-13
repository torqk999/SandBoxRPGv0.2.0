using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invulnerability", menuName = "ScriptableObjects/Effects/Invulnerability")]
public class InvulnerableEffect : BaseEffect
{
    [Header("Invulnerable Properties")]
    public RawStat TargetStat;

    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is InvulnerableEffect))
            return;

        InvulnerableEffect invulnerableSource = (InvulnerableEffect)source;

        TargetStat = invulnerableSource.TargetStat;
    }

    public override BaseEffect GenerateEffect(Character effected = null, bool inject = true)
    {
        InvulnerableEffect newEffect = (InvulnerableEffect)CreateInstance("InvulnerableEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
}
