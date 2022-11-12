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
    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is CrowdControlEffect))
            return;

        CrowdControlEffect immuneSource = (CrowdControlEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(bool inject = true)
    {
        CrowdControlEffect newEffect = (CrowdControlEffect)CreateInstance("ImmuneEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }

    public CrowdControlEffect(string name, CCstatus status, Sprite sprite = null) // Hard indefinite CC creation (ez death)
    {
        Name = name;
        Sprite = sprite;
        TargetCCstatus = status;
    }
}
