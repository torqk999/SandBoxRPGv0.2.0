using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Abilities/Passive")]
public class ToggleAbility : TargettedAbility
{
    [Header("Passive Properties")]
    public ParticleSystem Aura;
    //public bool Innate;
    public bool Active;

    public override void CloneEffects(TargettedAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneEffects(source, equipId, potency, inject);
    }

    public override void CloneAbility(CharacterAbility source, int equipId, float potency = 1, bool inject = false)
    {
        if (!(source is ToggleAbility))
            return;

        ToggleAbility passiveSource = (ToggleAbility)source;

        equipId = equipId == -1 ? 0 : equipId; // use zero as universal passive place-holder
        //Innate = passiveSource.Innate;
        Active = false;

        base.CloneAbility(source, equipId, potency, inject);
    }

    public override CharacterAbility GenerateAbility(float potency = 1, bool inject = false, int equipID = 0)
    {
        ToggleAbility newAbility = (ToggleAbility)CreateInstance("PassiveAbility");
        newAbility.CloneAbility(this, equipID, potency, inject);
        return newAbility;
    }
}
