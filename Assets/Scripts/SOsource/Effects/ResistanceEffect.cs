using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Res Adjustment", menuName = "ScriptableObjects/Effects/Res Adjustment")]
public class ResistanceEffect : StatEffect
{
    [Header("Resistance Properties")]
    public ElementPackage BaseResAdjustments;
    public ElementPackage AmpedResAdjustments;

    public override void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        base.ApplySingleEffect(target, cast, toggle); // Risidual proc

        if (!PeriodClock())
            return;

        target.UpdateResAdjust();
    }
    public override void RemoveRisidualEffect()
    {
        EffectedCharacter.UpdateResAdjust();
        base.RemoveRisidualEffect();
    }
    public override void Amplify(float amp)
    {
        AmpedResAdjustments.Amplify(BaseResAdjustments, amp);
    }
    public override void InitializeSource()
    {
        BaseResAdjustments.Initialize();
    }
    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is ResistanceEffect))
            return;

        ResistanceEffect currentStatEffect = (ResistanceEffect)source;

        BaseResAdjustments.Clone(currentStatEffect.BaseResAdjustments);
        AmpedResAdjustments.Clone(BaseResAdjustments);
    }
    public override BaseEffect GenerateEffect(Character effected = null, bool inject = true)
    {
        ResistanceEffect newEffect = (ResistanceEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
}
