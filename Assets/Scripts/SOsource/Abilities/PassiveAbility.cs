using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Abilities/Passive")]
public class PassiveAbility : TargettedAbility
{
    [Header("Passive Properties")]
    public ParticleSystem Aura;
    public bool Innate;

    public override void CloneAbility(CharacterAbility source, int equipId, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source);

        if (!(source is PassiveAbility))
            return;

        PassiveAbility passiveSource = (PassiveAbility)source;

        equipId = equipId == -1 ? 0 : equipId; // use zero as universal passive place-holder
        Innate = passiveSource.Innate;

        Effects = new BaseEffect[passiveSource.Effects.Length];

        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = passiveSource.Effects[i].GenerateEffect();//(PassiveEffect)CreateInstance("PassiveEffect");
            Effects[i].CloneEffect(passiveSource.Effects[i], equipId, potency, inject);
        }
    }

    public override CharacterAbility GenerateAbility(Character currentCharacter, bool inject, Equipment equip = null)
    {
        PassiveAbility newAbility = (PassiveAbility)CreateInstance("PassiveAbility");
        int id = equip == null ? -1 : equip.EquipID;
        newAbility.CloneAbility(this, id, currentCharacter.GeneratePotency(equip), inject);
        return newAbility;
    }
}
