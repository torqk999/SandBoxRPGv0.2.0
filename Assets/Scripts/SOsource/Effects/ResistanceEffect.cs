using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Res Adjustment", menuName = "ScriptableObjects/Effects/Res Adjustment")]
public class ResistanceEffect : StatEffect
{
    [Header("Resistance Properties")]
    public ElementPackage ResAdjustments;
    //public ElementPackage AmpedResAdjustments;

    public override void ApplySingleEffect(Character target, EffectOptions options, bool cast = false)
    {
        base.ApplySingleEffect(target, options, cast); // Risidual proc

        if (!PeriodClock())
            return;

        target.UpdateResAdjust();
    }
    public override void RemoveRisidualEffect()
    {
        Logic.EffectedCharacter.UpdateResAdjust();
        base.RemoveRisidualEffect();
    }
    public override void Amplify(float amp)
    {
        ResAdjustments.Amplify(amp);
    }
    public override void InitializeSource()
    {
        ResAdjustments.Initialize();
    }
    public override void CloneEffect(BaseEffect source, EffectOptions options)
    {
        base.CloneEffect(source, options);

        if (!(source is ResistanceEffect))
            return;

        ResistanceEffect currentStatEffect = (ResistanceEffect)source;

        ResAdjustments.Clone(currentStatEffect.ResAdjustments);
    }
    public override BaseEffect GenerateEffect(EffectOptions options, Character effected = null)
    {
        ResistanceEffect newEffect = (ResistanceEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, options);
        return newEffect;
    }
}
