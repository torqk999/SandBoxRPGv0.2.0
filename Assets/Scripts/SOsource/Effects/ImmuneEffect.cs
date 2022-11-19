using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Immunity", menuName = "ScriptableObjects/Effects/Immunity")]
public class ImmuneEffect : BaseEffect
{
    [Header("ImmuneProperties")]
    public CCstatus TargetCCstatus;

    public override void ApplySingleEffect(Character target, EffectOptions options, bool cast = false)
    {
        base.ApplySingleEffect(target, options, cast); // Risidual proc

        if (cast)
            Cleanse(target);
    }

    void Cleanse(Character target)
    {
        foreach (CrowdControlEffect ccEffect in target.Risiduals)
            if (ccEffect.TargetCCstatus == TargetCCstatus)
                Destroy(ccEffect);
    }

    public override void CloneEffect(BaseEffect source, EffectOptions options)
    {
        base.CloneEffect(source, options);

        if (!(source is ImmuneEffect))
            return;

        ImmuneEffect immuneSource = (ImmuneEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(EffectOptions options, Character effected = null)
    {
        ImmuneEffect newEffect = (ImmuneEffect)CreateInstance("ImmuneEffect");
        newEffect.CloneEffect(this, options);
        return newEffect;
    }
}