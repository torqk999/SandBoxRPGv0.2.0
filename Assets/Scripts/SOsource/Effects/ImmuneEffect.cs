using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Immunity", menuName = "ScriptableObjects/Effects/Immunity")]
public class ImmuneEffect : BaseEffect
{
    [Header("ImmuneProperties")]
    public CCstatus TargetCCstatus;

    public override void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        base.ApplySingleEffect(target, cast, toggle); // Risidual proc

        if (cast)
            Cleanse(target);
    }

    void Cleanse(Character target)
    {
        foreach (CrowdControlEffect ccEffect in target.Risiduals)
            if (ccEffect.TargetCCstatus == TargetCCstatus)
                Destroy(ccEffect);
    }

    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is ImmuneEffect))
            return;

        ImmuneEffect immuneSource = (ImmuneEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(Character effected = null, bool inject = true)
    {
        ImmuneEffect newEffect = (ImmuneEffect)CreateInstance("ImmuneEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
}