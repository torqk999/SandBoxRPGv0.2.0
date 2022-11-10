using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Abilities/Passive")]
public class PassiveAbility : TargettedAbility
{
    [Header("Passive Properties")]
    //public new PassiveEffect[] Effects;
    public ParticleSystem Aura;
    public bool Innate;

    public override void CloneAbility(CharacterAbility source, int equipId, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source);

        if (!(source is PassiveAbility))
            return;

        PassiveAbility passiveSource = (PassiveAbility)source;

        Innate = passiveSource.Innate;

        Effects = new BaseEffect[passiveSource.Effects.Length];

        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = CreateEffectInstance(passiveSource.Effects[i]);//(PassiveEffect)CreateInstance("PassiveEffect");
            Effects[i].CloneEffect(passiveSource.Effects[i], equipId, potency, inject);
        }
    }
    public override CharacterAbility EquipAbility(Character currentCharacter, Equipment equip, bool inject)
    {
        PassiveAbility newPassive = (PassiveAbility)CreateInstance("PassiveAbility");
        newPassive.CloneAbility(this, equip.EquipID, GeneratePotency(currentCharacter, equip), inject);
        if (newPassive.Innate)
        {
            foreach (BaseEffect passiveEffect in newPassive.Effects)
                currentCharacter.AddRisidiualEffect(passiveEffect, newPassive.EquipID);
        }

        return newPassive;
    }
}
