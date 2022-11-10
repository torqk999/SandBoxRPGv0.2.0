using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage&Heal", menuName = "ScriptableObjects/Effects/Damage&Heal")]
public class CurrentStatEffect : StatEffect
{
    [Header("Damage Values")]
    public RawStat TargetStat;
    public ElementPackage ElementPack;

    public override void CloneEffect(BaseEffect source, int equipId = -1, float amp = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId, amp, inject);

        if (!(source is CurrentStatEffect))
            return;

        CurrentStatEffect currentStatEffect = (CurrentStatEffect)source;

        ElementPack = new ElementPackage(currentStatEffect.ElementPack);
        ElementPack.Reflection.Reflect(ref ElementPack.Elements, inject);
        ElementPack.Amplify(amp);
    }

    public CurrentStatEffect(RawStat targetStat, float magnitude) // Default regen
    {
        Name = $"{targetStat} REGEN";
        TargetStat = targetStat;
        Value = ValueType.FLAT;
        //Action = EffectAction.DMG_HEAL;
        ElementPack = new ElementPackage();
        ElementPack.Init();
        ElementPack.Elements[(int)Element.HEALING] = magnitude;
    }
}
