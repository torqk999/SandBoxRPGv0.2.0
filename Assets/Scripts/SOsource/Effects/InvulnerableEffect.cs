using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Invulnerability", menuName = "ScriptableObjects/Effects/Invulnerability")]
public class InvulnerableEffect : BaseEffect
{
    [Header("Invulnerable Properties")]
    public RawStat TargetStat;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId);

        if (!(source is InvulnerableEffect))
            return;

        InvulnerableEffect invulnerableSource = (InvulnerableEffect)source;

        TargetStat = invulnerableSource.TargetStat;
    }

    public override BaseEffect GenerateEffect(int equipId = -1, float potency = 1, bool inject = true)
    {
        InvulnerableEffect newEffect = (InvulnerableEffect)CreateInstance("InvulnerableEffect");
        newEffect.CloneEffect(this, equipId);
        return newEffect;
    }
}
