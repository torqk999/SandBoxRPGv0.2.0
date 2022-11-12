using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ToggleAbility", menuName = "ScriptableObjects/Abilities/Toggle")]
public class ToggleAbility : TargettedAbility
{
    [Header("Toggle Properties")]
    public ParticleSystem Aura;
    public bool Active;

    public override void UseAbility(Character target)
    {
        Active = !Active;

        for (int i = 0; i < Effects.Length; i++)
            Effects[i].ApplySingleEffect(target, true, Active);
    }

    public override void CloneEffects(TargettedAbility source, bool inject = false)
    {
        base.CloneEffects(source, inject);
    }

    public override void CloneAbility(CharacterAbility source, bool inject = false)
    {
        if (!(source is ToggleAbility))
            return;

        ToggleAbility passiveSource = (ToggleAbility)source;

        Aura = passiveSource.Aura;
        Active = false;

        base.CloneAbility(source, inject);
    }

    public override CharacterAbility GenerateAbility(bool inject = false)
    {
        ToggleAbility newAbility = (ToggleAbility)CreateInstance("PassiveAbility");
        newAbility.CloneAbility(this, inject);
        return newAbility;
    }
}
