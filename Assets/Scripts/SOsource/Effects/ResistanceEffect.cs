using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Res Adjustment", menuName = "ScriptableObjects/Effects/Res Adjustment")]
public class ResistanceEffect : StatEffect
{
    [Header("Resistance Properties")]
    public ElementPackage ResAdjustments;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId, potency, inject);

        if (!(source is ResistanceEffect))
            return;

        ResistanceEffect currentStatEffect = (ResistanceEffect)source;

        ResAdjustments.Clone(currentStatEffect.ResAdjustments);
        ResAdjustments.Reflection.Reflect(ref ResAdjustments.Elements, inject);
        ResAdjustments.Amplify(potency);
    }

    public override BaseEffect GenerateEffect(float potency = 1, bool inject = true, int equipId = -1)
    {
        ResistanceEffect newEffect = (ResistanceEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, equipId, potency, inject);
        return newEffect;
    }
}
