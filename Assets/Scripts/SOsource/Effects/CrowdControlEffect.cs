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
    public override void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        base.CloneEffect(source, effectOptions);

        if (!(source is CrowdControlEffect))
            return;

        CrowdControlEffect immuneSource = (CrowdControlEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "CrowdControlEffect";
        return (CrowdControlEffect)base.GenerateRootObject(options);
    }

    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        CrowdControlEffect newEffect = (CrowdControlEffect)base.GenerateEffect(rootOptions, effectOptions, effected);
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }
}
