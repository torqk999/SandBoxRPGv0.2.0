using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Immunity", menuName = "ScriptableObjects/Effects/Immunity")]
public class ImmuneEffect : BaseEffect
{
    [Header("ImmuneProperties")]
    public CCstatus TargetCCstatus;

    public override void ApplySingleEffect(Character target, bool cast = false)
    {
        base.ApplySingleEffect(target, cast); // Risidual proc

        if (cast)
            Cleanse(target);
    }

    void Cleanse(Character target)
    {
        for (int i = target.Slots.Risiduals.Count - 1; i > -1; i--)
            if (target.Slots.Risiduals[i] is CrowdControlEffect && ((CrowdControlEffect)target.Slots.Risiduals[i]).TargetCCstatus == TargetCCstatus)
            {
                Destroy(target.Slots.Risiduals[i]);
                target.Slots.Risiduals.RemoveAt(i);
            }
    }

    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is ImmuneEffect))
            return;

        ImmuneEffect immuneSource = (ImmuneEffect)options.Source;
        TargetCCstatus = immuneSource.TargetCCstatus;
    }
    /*public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "ImmuneEffect";
        return (ImmuneEffect)base.GenerateRootObject(options);
    }
    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        ImmuneEffect newEffect = (ImmuneEffect)base.GenerateEffect(rootOptions, effectOptions, effected);
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }*/
}