using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Immunity", menuName = "ScriptableObjects/Effects/Immunity")]
public class ImmuneEffect : BaseEffect
{
    [Header("ImmuneProperties")]
    public CCstatus TargetCCstatus;

    public override void CloneEffect(BaseEffect source, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        base.CloneEffect(source, ability, equip, inject);

        if (!(source is ImmuneEffect))
            return;

        ImmuneEffect immuneSource = (ImmuneEffect)source;

        TargetCCstatus = immuneSource.TargetCCstatus;
    }

    public override BaseEffect GenerateEffect(CharacterAbility ability = null, Equipment equip = null, bool inject = true)
    {
        ImmuneEffect newEffect = (ImmuneEffect)CreateInstance("ImmuneEffect");
        newEffect.CloneEffect(this, ability, equip, inject);
        return newEffect;
    }
}