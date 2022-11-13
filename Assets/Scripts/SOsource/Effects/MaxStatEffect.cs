using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public RawStatPackage BaseStatAdjustPack;
    public RawStatPackage AmpedStatAdjustPack;

    public override void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        base.ApplySingleEffect(target, cast, toggle); // Risidual proc
    }
    public override void Amplify(float amp)
    {
        AmpedStatAdjustPack.Amplify(BaseStatAdjustPack, amp);
    }
    public override void InitializeSource()
    {
        BaseStatAdjustPack.Initialize();
    }
    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        BaseStatAdjustPack.Clone(maxSource.BaseStatAdjustPack);
        AmpedStatAdjustPack.Clone(BaseStatAdjustPack);
    }
    public override BaseEffect GenerateEffect(Character effected = null, bool inject = true)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
}
