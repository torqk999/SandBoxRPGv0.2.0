using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage&Heal", menuName = "ScriptableObjects/Effects/Damage&Heal")]
public class CurrentStatEffect : StatEffect
{
    [Header("Damage Values")]
    public RawStat TargetStat;
    public ElementPackage ElementPack;

    public override void ApplySingleEffect(Character target, EffectOptions options, bool cast = false)
    {
        base.ApplySingleEffect(target, options, cast); // Risidual proc

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
            float change = (damageModifier * ElementPack.Amplification[i]) * (1 - (res / (res + CharacterMath.RES_PRIME_DENOM)));
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
        ElementPack.Amplify(amp);
    }
    public override void InitializeRoot(GameState state)
    {
        ElementPack.Initialize();
    }
    public override void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        base.CloneEffect(source, effectOptions);

        if (!(source is CurrentStatEffect))
            return;

        CurrentStatEffect currentStatEffectSource = (CurrentStatEffect)source;
        TargetStat = currentStatEffectSource.TargetStat;

        ElementPack.Clone(currentStatEffectSource.ElementPack);
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "CurrentStatEffect";
        return (CurrentStatEffect)base.GenerateRootObject(options);
    }
    public override BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        CurrentStatEffect newEffect = (CurrentStatEffect)base.GenerateEffect(rootOptions, effectOptions, effected);
        newEffect.CloneEffect(this, effectOptions);
        return newEffect;
    }
    public void FormRegen(RawStat targetStat, float healMagnitude) // Default regen
    {
        Name = $"{targetStat} REGEN";
        TargetStat = targetStat;
        Value = ValueType.FLAT;
        ElementPack = new ElementPackage(healMagnitude);
    }
}
