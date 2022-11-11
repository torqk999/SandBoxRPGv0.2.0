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

    public override void CloneEffects(TargettedAbility source, Equipment equip = null, bool inject = false)
    {
        base.CloneEffects(source, equip, inject);
    }

    public override void CloneAbility(CharacterAbility source, Equipment equip = null, bool inject = false)
    {
        if (!(source is ToggleAbility))
            return;

        ToggleAbility passiveSource = (ToggleAbility)source;

        //equipId = equipId == -1 ? 0 : equipId; // use zero as universal passive place-holder
        //Innate = passiveSource.Innate;
        Aura = passiveSource.Aura;
        Active = false;

        base.CloneAbility(source, equip, inject);
    }

    public override CharacterAbility GenerateAbility(Equipment equip = null, bool inject = false)
    {
        ToggleAbility newAbility = (ToggleAbility)CreateInstance("PassiveAbility");
        newAbility.CloneAbility(this, equip, inject);
        return newAbility;
    }
}
