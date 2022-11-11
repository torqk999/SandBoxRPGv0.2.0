using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Res Adjustment", menuName = "ScriptableObjects/Effects/Res Adjustment")]
public class ResistanceEffect : StatEffect
{
    [Header("Resistance Properties")]
    public ElementPackage ResAdjustments;

    public override void CloneEffect(BaseEffect source, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        base.CloneEffect(source, ability, equip, inject, amp);

        if (!(source is ResistanceEffect))
            return;

        ResistanceEffect currentStatEffect = (ResistanceEffect)source;

        ResAdjustments.Clone(currentStatEffect.ResAdjustments);
        ResAdjustments.Reflection.Reflect(ref ResAdjustments.Elements, inject);
        if (amp)
        ResAdjustments.Amplify(CharacterMath.GeneratePotency());
    }

    public override BaseEffect GenerateEffect(CharacterAbility ability = null, Equipment equip = null, bool inject = true, bool amp = false)
    {
        ResistanceEffect newEffect = (ResistanceEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, ability, equip, inject);
        return newEffect;
    }
}
