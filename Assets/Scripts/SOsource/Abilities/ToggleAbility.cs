using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ToggleAbility", menuName = "ScriptableObjects/Abilities/Toggle")]
public class ToggleAbility : EffectAbility
{
    [Header("Toggle Properties")]
    public ParticleSystem Aura;
    public bool Active;

    public override void UseAbility(Character target, EffectOptions options = default(EffectOptions))
    {
        Active = !Active;

        options.EffectType = EffectType.TOGGLE;
        options.ToggleActive = Active;
        options.IsProjectile = true;
        options.IsClone = true;
        options.Inject = false;

        base.UseAbility(target, options);
    }

    public override void ProduceOriginalEffects(EffectAbility source, bool inject = false)
    {
        base.ProduceOriginalEffects(source, inject);
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
