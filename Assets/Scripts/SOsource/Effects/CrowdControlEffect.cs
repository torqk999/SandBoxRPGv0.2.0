using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrowdControl", menuName = "ScriptableObjects/Effects/CrowdControl")]
public class CrowdControlEffect : BaseEffect
{
    [Header("CC Properties")]
    public CCstatus TargetCCstatus;

    public override void CloneEffect(BaseEffect source, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        base.CloneEffect(source, ability, equip, inject);

        if (!(source is CrowdControlEffect))
            return;

        CrowdControlEffect immuneSource = (CrowdControlEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(CharacterAbility ability = null, Equipment equip = null, bool inject = true, bool amp = false)
    {
        CrowdControlEffect newEffect = (CrowdControlEffect)CreateInstance("ImmuneEffect");
        newEffect.CloneEffect(this, ability, equip, inject);
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
