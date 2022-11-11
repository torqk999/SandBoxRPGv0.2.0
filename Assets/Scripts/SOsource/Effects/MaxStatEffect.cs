using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stat Adjustment", menuName = "ScriptableObjects/Effects/Stat Adjustment")]
public class MaxStatEffect : StatEffect
{
    [Header ("Adjust Values")]
    public RawStatPackage StatAdjustPack;

    public override void CloneEffect(BaseEffect source, CharacterSheet sheet = null, CharacterAbility ability = null, Equipment equip = null, bool inject = false)
    {
        base.CloneEffect(source, ability);

        if (!(source is MaxStatEffect))
            return;

        MaxStatEffect maxSource = (MaxStatEffect)source;

        StatAdjustPack.Clone(maxSource.StatAdjustPack); //= new StatPackage(maxSource.StatAdjustPack);
        StatAdjustPack.Reflect(inject);
        StatAdjustPack.Amplify(CharacterMath.GeneratePotency(null, equip));
    }

    public override BaseEffect GenerateEffect(CharacterAbility ability = null, Equipment equip = null, bool inject = true)
    {
        MaxStatEffect newEffect = (MaxStatEffect)CreateInstance("MaxStatEffect");
        newEffect.CloneEffect(this, ability, equip, inject);
        return newEffect;
    }
}
