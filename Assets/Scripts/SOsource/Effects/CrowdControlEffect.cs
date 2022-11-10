using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdControl", menuName = "ScriptableObjects/Effects/CrowdControl")]
public class CrowdControlEffect : BaseEffect
{
    [Header("CC Properties")]
    public CCstatus TargetCCstatus;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId);

        if (!(source is CrowdControlEffect))
            return;

        CrowdControlEffect immuneSource = (CrowdControlEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(int equipId = -1, float potency = 1, bool inject = true)
    {
        CrowdControlEffect newEffect = (CrowdControlEffect)CreateInstance("ImmuneEffect");
        newEffect.CloneEffect(this, equipId);
        return newEffect;
    }

    public CrowdControlEffect(string name, CCstatus status, Sprite sprite = null) // Hard indefinite CC creation (ez death)
    {
        Name = name;
        Sprite = sprite;
        EquipID = 0;
        TargetCCstatus = status;
    }
}
