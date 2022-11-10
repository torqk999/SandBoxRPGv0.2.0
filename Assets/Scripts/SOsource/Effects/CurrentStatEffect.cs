using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage&Heal", menuName = "ScriptableObjects/Effects/Damage&Heal")]
public class CurrentStatEffect : StatEffect
{
    [Header("Damage Values")]
    public RawStat TargetStat;
    public ElementPackage ElementPack;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId, potency, inject);

        if (!(source is CurrentStatEffect))
            return;

        CurrentStatEffect currentStatEffect = (CurrentStatEffect)source;

        ElementPack = new ElementPackage(currentStatEffect.ElementPack);
        ElementPack.Reflect(inject);
        ElementPack.Amplify(potency);
    }

    public override BaseEffect GenerateEffect(int equipId = -1, float potency = 1, bool inject = true)
    {
        CurrentStatEffect newEffect = (CurrentStatEffect)CreateInstance("CurrentStatEffect");
        newEffect.CloneEffect(this, equipId, potency, inject);
        return newEffect;
    }

    public CurrentStatEffect(RawStat targetStat, float magnitude) // Default regen
    {
        Name = $"{targetStat} REGEN";
        TargetStat = targetStat;
        Value = ValueType.FLAT;
        ElementPack = new ElementPackage(magnitude);
    }
}
