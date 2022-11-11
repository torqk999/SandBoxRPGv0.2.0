using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage&Heal", menuName = "ScriptableObjects/Effects/Damage&Heal")]
public class CurrentStatEffect : StatEffect
{
    [Header("Damage Values")]
    public RawStat TargetStat;
    public ElementPackage ElementPack;

    public override void ApplySingleEffect(Character target, CharacterAbility ability = null, Equipment equip = null, bool cast = false, bool toggle = true)
    {
        base.ApplySingleEffect(target, ability, equip, cast, toggle);

        if (target.CheckDamageImmune(TargetStat))
            return;

        float totalDamage = 0;
        float damageAmount = target.GenerateStatValueModifier(Value, TargetStat);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            //if (CheckElementalImmune((Element)i))
            //continue;
            float res = target.CurrentResistances.Elements[i];
            float change = (damageAmount * ElementPack.Elements[i]) * (1 - (res / (res + CharacterMath.RES_PRIME_DENOM)));
            totalDamage += (Element)i == Element.HEALING ? -change : change; // Everything but healing
        }

        if (totalDamage == 0)
            return;

        float[] stats = target.CurrentStats.Stats;
        float[] max = target.MaximumStatValues.Stats;

        stats[(int)TargetStat] -= totalDamage;
        stats[(int)TargetStat] = stats[(int)TargetStat] <= max[(int)TargetStat] ? stats[(int)TargetStat] : max[(int)TargetStat];
        stats[(int)TargetStat] = stats[(int)TargetStat] >= 0 ? stats[(int)TargetStat] : 0;

        // Debugging
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

                break;

            case RawStat.STAMINA:
                if (totalDamage > 0)
                    target.DebugState = DebugState.LOSS_S;
                break;
        }
    }

    public override void CloneEffect(BaseEffect source, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        base.CloneEffect(source, ability, equip, inject);

        if (!(source is CurrentStatEffect))
            return;

        CurrentStatEffect currentStatEffect = (CurrentStatEffect)source;

        ElementPack.Clone(currentStatEffect.ElementPack); //= new ElementPackage(currentStatEffect.ElementPack);
        ElementPack.Reflect(inject);
        ElementPack.Amplify(potency);
    }

    public override BaseEffect GenerateEffect(CharacterAbility ability = null, Equipment equip = null, bool inject = true)
    {
        CurrentStatEffect newEffect = (CurrentStatEffect)CreateInstance("CurrentStatEffect");
        newEffect.CloneEffect(this, ability, equip, inject);
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
