using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Abilities/Passive")]
public class PassiveAbility : CharacterAbility
{
    [Header("Proc Properties")]
    public ParticleSystem AuraSelf;
    public ParticleSystem AuraAOE;
    public BaseEffect[] Passives;
    public float Timer;
    public float Period;

    public void ClonePassives(PassiveAbility source, bool inject = false)
    {
        Passives = new BaseEffect[source.Passives.Length];
        for (int i = 0; i < Passives.Length; i++)
            Passives[i] = source.Passives[i].GenerateEffect(inject);
    }

    public override void CloneAbility(CharacterAbility source, bool inject = false)
    {
        if (!(source is PassiveAbility))
            return;

        PassiveAbility passiveSource = (PassiveAbility)source;

        AuraSelf = passiveSource.AuraSelf;

        base.CloneAbility(source, inject);
    }

    public override CharacterAbility GenerateAbility(bool inject = false)
    {
        PassiveAbility newAbility = (PassiveAbility)CreateInstance("PassiveAbility");
        newAbility.CloneAbility(this, inject);
        return newAbility;
    }
}
