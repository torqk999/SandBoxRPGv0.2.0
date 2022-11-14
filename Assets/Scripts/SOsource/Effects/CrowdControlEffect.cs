using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdControl", menuName = "ScriptableObjects/Effects/CrowdControl")]
public class CrowdControlEffect : BaseEffect
{
    [Header("CC Properties")]
    public CCstatus TargetCCstatus;

    /*public override void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        base.ApplySingleEffect(target, cast, toggle); // Risidual proc

        if (target.CheckCCimmune(TargetCCstatus))
            return;

        if (!PeriodClock())
            return;

        target.Risiduals.Add(G)
    }*/
    public override void CloneEffect(BaseEffect source, EffectOptions options)
    {
        base.CloneEffect(source, options);

        if (!(source is CrowdControlEffect))
            return;

        CrowdControlEffect immuneSource = (CrowdControlEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(EffectOptions options, Character effected = null)
    {
        CrowdControlEffect newEffect = (CrowdControlEffect)CreateInstance("CrowdControlEffect");
        newEffect.CloneEffect(this, options);
        return newEffect;
    }
}
