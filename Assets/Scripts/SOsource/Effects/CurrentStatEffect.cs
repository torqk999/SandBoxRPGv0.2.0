using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage&Heal", menuName = "ScriptableObjects/Effects/Damage&Heal")]
public class CurrentStatEffect : StatEffect
{
    [Header("Damage Values")]
    public RawStat TargetStat;
    public ElementPackage BaseElementPack;
    public ElementPackage AmpedElementPack;

    public override void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        base.ApplySingleEffect(target, cast, toggle); // Risidual proc

        if (target.CheckDamageImmune(TargetStat))
            return;

        if (!PeriodClock())
            return;

        Damage(target);
        //DamageDebug(target, Damage(target));
    }
    float Damage(Character target)
    {
        float totalDamage = 0;
        float damageModifier = target.GenerateRawStatValueModifier(Value, TargetStat);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            //if (CheckElementalImmune((Element)i))
            //continue;
            float res = target.CurrentResistances.Elements[i];
            float change = (damageModifier * AmpedElementPack.Elements[i]) * (1 - (res / (res + CharacterMath.RES_PRIME_DENOM)));
            totalDamage += (Element)i == Element.HEALING ? -change : change; // Everything but healing
        }

        if (totalDamage == 0)
            return 0;

        float[] stats = target.CurrentStats.Stats;
        float[] max = target.MaximumStatValues.Stats;

        stats[(int)TargetStat] -= totalDamage;
        stats[(int)TargetStat] = stats[(int)TargetStat] <= max[(int)TargetStat] ? stats[(int)TargetStat] : max[(int)TargetStat];
        stats[(int)TargetStat] = stats[(int)TargetStat] >= 0 ? stats[(int)TargetStat] : 0;

        return totalDamage;
    }
    void DamageDebug(Character target, float totalDamage)
    {
        target.bAssetUpdate = true;

        switch (TargetStat)
        {
            case RawStat.HEALTH:
                if (totalDamage > 0)
                    target.DebugState = DebugState.LOSS_H;
                break;

            case RawStat.MANA:
                if (totalDamage > 0)
                    target.DebugState = DebugState.LOSS_M;
                break;

            case RawStat.SPEED:
                // custom speed adjust logic? haste, slow, decay?
                break;

            case RawStat.STAMINA:
                if (totalDamage > 0)
                    target.DebugState = DebugState.LOSS_S;
                break;
        }
    }
    public override void Amplify(float amp)
    {
        AmpedElementPack.Amplify(BaseElementPack, amp);
    }
    public override void InitializeSource()
    {
        BaseElementPack.Initialize();
    }
    public override void CloneEffect(BaseEffect source, bool inject = false)
    {
        base.CloneEffect(source, inject);

        if (!(source is CurrentStatEffect))
            return;

        CurrentStatEffect currentStatEffectSource = (CurrentStatEffect)source;
        TargetStat = currentStatEffectSource.TargetStat;

        BaseElementPack.Clone(currentStatEffectSource.BaseElementPack);
        AmpedElementPack.Clone(BaseElementPack);
    }
    public override BaseEffect GenerateEffect(bool inject = true)
    {
        CurrentStatEffect newEffect = (CurrentStatEffect)CreateInstance("CurrentStatEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
    public void FormRegen(RawStat targetStat, float healMagnitude) // Default regen
    {
        Name = $"{targetStat} REGEN";
        TargetStat = targetStat;
        Value = ValueType.FLAT;
        BaseElementPack = new ElementPackage(healMagnitude);
    }
}
