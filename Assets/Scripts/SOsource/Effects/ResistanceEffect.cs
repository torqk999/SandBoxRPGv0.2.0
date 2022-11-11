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
        base.CloneEffect(source, sheet, ability, equip, inject);

        if (!(source is ResistanceEffect))
            return;

        ResistanceEffect currentStatEffect = (ResistanceEffect)source;

        ResAdjustments.Clone(currentStatEffect.ResAdjustments);
        ResAdjustments.Reflection.Reflect(ref ResAdjustments.Elements, inject);
        ResAdjustments.Amplify(CharacterMath.GeneratePotency());
    }

    public override BaseEffect GenerateEffect(CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = true)
    {
        ResistanceEffect newEffect = (ResistanceEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, sheet, ability, equip, inject);
        return newEffect;
    }
}
