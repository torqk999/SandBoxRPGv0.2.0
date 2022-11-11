using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage&Heal", menuName = "ScriptableObjects/Effects/Damage&Heal")]
public class CurrentStatEffect : StatEffect
{
    [Header("Damage Values")]
    public RawStat TargetStat;
    public ElementPackage ElementPack;

    public override void ApplySingleEffect(Character character, bool cast = false, int equipId = -1)
    {
        base.ApplySingleEffect(character, cast, equipId);

        if (character.CheckDamageImmune(TargetStat))
            return;

        float totalDamage = 0;
        float damageAmount = character.GenerateStatValueModifier(Value, TargetStat);

        for (int i = 0; i < CharacterMath.STATS_ELEMENT_COUNT; i++)
        {
            //if (CheckElementalImmune((Element)i))
            //continue;
            float res = character.CurrentResistances.Elements[i];
            float change = (damageAmount * ElementPack.Elements[i]) * (1 - (res / (res + CharacterMath.RES_PRIME_DENOM)));
            totalDamage += (Element)i == Element.HEALING ? -change : change; // Everything but healing
        }

        if (totalDamage == 0)
            return;

        float[] stats = character.CurrentStats.Stats;
        float[] max = character.MaximumStatValues.Stats;

        stats[(int)TargetStat] -= totalDamage;
        stats[(int)TargetStat] = stats[(int)TargetStat] <= max[(int)TargetStat] ? stats[(int)TargetStat] : max[(int)TargetStat];
        stats[(int)TargetStat] = stats[(int)TargetStat] >= 0 ? stats[(int)TargetStat] : 0;

        // Debugging
        character.bAssetUpdate = true;

        switch (TargetStat)
        {
            case RawStat.HEALTH:
                if (totalDamage > 0)
                    character.DebugState = DebugState.LOSS_H;
                break;

            case RawStat.MANA:
                if (totalDamage > 0)
                    character.DebugState = DebugState.LOSS_M;
                break;

            case RawStat.SPEED:

                break;

            case RawStat.STAMINA:
                if (totalDamage > 0)
                    character.DebugState = DebugState.LOSS_S;
                break;
        }
    }

    public override void CloneEffect(BaseEffect source, int equipId = -1, float potency = 1, bool inject = true)
    {
        base.CloneEffect(source, equipId);

        if (!(source is CurrentStatEffect))
            return;

        CurrentStatEffect currentStatEffect = (CurrentStatEffect)source;

        ElementPack.Clone(currentStatEffect.ElementPack); //= new ElementPackage(currentStatEffect.ElementPack);
        ElementPack.Reflect(inject);
        ElementPack.Amplify(potency);
    }

    public override BaseEffect GenerateEffect(int equipId = -1, float potency = 1, bool inject = true)
    {
        CurrentStatEffect newEffect = (CurrentStatEffect)CreateInstance("CurrentStatEffect");
        newEffect.CloneEffect(this, equipId, potency, inject);
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
