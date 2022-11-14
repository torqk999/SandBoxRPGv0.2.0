using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveAbility", menuName = "ScriptableObjects/Abilities/Passive")]
public class PassiveAbility : EffectAbility
{
    [Header("Proc Properties")]
    public ParticleSystem AuraSelf;
    public ParticleSystem AuraAOE;
    //public BaseEffect[] Passives;
    public float ProcTimer;
    public float ProcDelay;

    public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        options.EffectType = EffectType.PASSIVE;
        options.ToggleActive = true;
        options.IsProjectile = true;
        options.IsClone = true;
        options.Inject = false;

        base.UseAbility(target, options);
    }
    public override void CloneAbility(CharacterAbility source, bool inject = false)
    {
        if (!(source is PassiveAbility))
            return;

        PassiveAbility passiveSource = (PassiveAbility)source;

        AuraSelf = passiveSource.AuraSelf;
        AuraAOE = passiveSource.AuraAOE;

        //ClonePassives(passiveSource, inject);

        ProcDelay = passiveSource.ProcDelay;
        ProcTimer = ProcDelay;

        base.CloneAbility(source, inject);
    }
    public override CharacterAbility GenerateAbility(bool inject = false)
    {
        PassiveAbility newAbility = (PassiveAbility)CreateInstance("PassiveAbility");
        newAbility.CloneAbility(this, inject);
        return newAbility;
    }
    public override void UpdatePassiveTimer()
    {
        ProcTimer -= GlobalConstants.TIME_SCALE;
        if (ProcTimer <= 0)
        {
            ProcTimer = ProcDelay;
            PassiveProc();
        }
    }
    void PassiveProc()
    {
        if (Logic.SourceCharacter == null)
        {
            Debug.Log("No source!");
            return;
        }

        Logic.SourceCharacter.AttemptAbility(this);
    }
}
