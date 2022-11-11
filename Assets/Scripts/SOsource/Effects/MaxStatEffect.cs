using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public StatPackage StatAdjustPack;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        StatAdjustPack.Clone(maxSource.StatAdjustPack); //= new StatPackage(maxSource.StatAdjustPack);
        StatAdjustPack.Reflect(inject);
        StatAdjustPack.Amplify(potency);
    }

    public override BaseEffect GenerateEffect(float potency = 1, bool inject = true, int equipId = -1)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, equipId, potency, inject);
        return newEffect;
    }
}
