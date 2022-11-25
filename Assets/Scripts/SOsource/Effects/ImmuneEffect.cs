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

    public override void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        base.CloneEffect(source, effectOptions);

        if (!(source is ImmuneEffect))
            return;

        ImmuneEffect immuneSource = (ImmuneEffect)source;
        TargetCCstatus = immuneSource.TargetCCstatus;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "ImmuneEffect";
        return (ImmuneEffect)base.GenerateRootObject(options);
    }
    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        ImmuneEffect newEffect = (ImmuneEffect)base.GenerateEffect(rootOptions, effectOptions, effected);
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }
}