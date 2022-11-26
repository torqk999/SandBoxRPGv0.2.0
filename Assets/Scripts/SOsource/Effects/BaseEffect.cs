using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    NONE,
    PASSIVE,
    PROC,
    TOGGLE
}

public class BaseEffect : RootScriptObject
{
    [Header("Base Properties")]
    public PsystemType ProjectileType;
    public TargetType Target;
    public float AOE_Range;

    public float ProjectileDuration;
    public float PeriodDuration;
    public float RisidualDuration;

    [Header("NO TOUCHY!")]
    public EffectLogic Logic;

    public Character SourceCharacter() // Because I am le lazy...
    {
        try
        {
            return Logic.Options.Source.Logic.SourceCharacter;
        }
        catch
        {
            Debug.Log($"Bad SourceCharacter reference on effect: {Name}");
            return null;
        }
    }
    public bool HasEligableTarget()
    {
        if (RootLogic.Options.GameState == null)
            return false;

        if (SourceCharacter() == null)
            return false;

        if (Target == TargetType.SELF)
            return true;

        if (AOE_Range <= 0)
            return false;

        if (RootLogic.Options.GameState.CharacterMan.CharacterPool == null ||
            RootLogic.Options.GameState.CharacterMan.CharacterPool.Count < 1)
            return false;

        List<Character> pool = RootLogic.Options.GameState.CharacterMan.CharacterPool;

        foreach(Character character in pool)
        {
            if (Target == TargetType.ALLY && !SourceCharacter().CheckAllegiance(character))
                return false;
            if (Target == TargetType.ENEMY && SourceCharacter().CheckAllegiance(character))
                return false;
            if (Vector3.Distance(SourceCharacter().Root.position, character.Root.position) <= AOE_Range)
                return true;
        }

        return false;
    }
    public bool PeriodClock(bool reset = false)
    {
        if (reset)
        {
            Logic.PeriodTimer = 0;
            return true;
        }

        if (Logic.PeriodTimer != 0)
            Logic.PeriodTimer -= GlobalConstants.TIME_SCALE;

        if (Logic.PeriodTimer < 0)
            Logic.PeriodTimer = 0;

        return Logic.PeriodTimer == 0;
    }
    public bool DurationClock(bool reset = false)
    {
        if (Logic.Options.EffectType != EffectType.PROC)
            return false;

        if (reset)
        {
            Logic.DurationTimer = RisidualDuration;
            return true;
        }

        if (Logic.DurationTimer != 0)
            Logic.DurationTimer -= GlobalConstants.TIME_SCALE;

        if (Logic.DurationTimer < 0)
            Logic.DurationTimer = 0;

        return Logic.DurationTimer == 0;
    }
    public bool ProjectileClock(bool reset = false, PsystemType projectile = PsystemType.NONE)
    {
        if (reset)
        {
            Logic.ProjectileTimer = ProjectileDuration;
            return true;
        }

        if (Logic.ProjectileTimer != 0)
            Logic.ProjectileTimer -= GlobalConstants.TIME_SCALE;

        if (Logic.ProjectileTimer < 0)
            Logic.ProjectileTimer = 0;

        ProjectilePsystem(Logic.ProjectileTimer / ProjectileDuration, projectile);

        return Logic.ProjectileTimer == 0;
    }
    void ProjectilePsystem(float lerp, PsystemType projectile = PsystemType.NONE)
    {
        Debug.Log("Stepped into Projectile system");

        if (projectile == PsystemType.NONE)
            return;

        if (Logic.Options.Source == null ||
            Logic.Options.Source.Logic.SourceCharacter == null ||
            Logic.Options.Effected == null)
            return;

        Debug.Log("Firing...");

        if (lerp == 0)
        {
            Destroy(Logic.Projectile);
            return;
        }
        if (Logic.Projectile == null)
        {
            if (RootLogic.Options.GameState == null)
                return;

            Logic.Projectile = Instantiate(RootLogic.Options.GameState.SceneMan.PsystemPrefabs[(int)ProjectileType], Logic.Options.Source.Logic.SourceCharacter.transform);
            Logic.Projectile.transform.localPosition = Vector3.zero;
        }
        //Projectile.transform.rotation = Quaternion.Lerp(EffectedCharacter.Root.rotation, SourceAbility.SourceCharacter.Root.rotation, lerp);
        Logic.Projectile.transform.LookAt(Logic.Options.Effected.Root);
        Logic.Projectile.transform.position = Vector3.Lerp(Logic.Options.Effected.Root.position, Logic.Options.Source.Logic.SourceCharacter.Root.position, lerp);
    }
    public virtual void ApplySingleEffect(Character target, bool cast = false)
    {
        if (cast)
        {
            if (ResetExistingRisiduals(target))
                return;

            if (ProjectileDuration > 0 ||
                RisidualDuration > 0 ||
                Logic.Options.EffectType == EffectType.PASSIVE ||
                Logic.Options.EffectType == EffectType.TOGGLE)
            {
                GenerateRisidualEffect(target, ProjectileType);
            }
        }

        if (!cast)
        {
            if (!ProjectileClock(false, ProjectileType))
                return;

            //if (Logic.Options.EffectType == EffectType.TOGGLE)
                //Logic.ToggleActive = !Logic.ToggleActive;

            if (Logic.Projectile != null)
                Destroy(Logic.Projectile);

            if (DurationClock() &&
                Logic.Options.EffectType == EffectType.PROC)
                RemoveRisidualEffect();
        }
    }
    bool ResetExistingRisiduals(Character target)
    {
        foreach (BaseEffect effect in target.Slots.Risiduals)
        {
            if (RootLogic.Clones.Find(x => x == effect) != null) // Pre-existing generated effect
            {
                switch (Logic.Options.EffectType)
                {
                    case EffectType.TOGGLE:
                        effect.Logic.ToggleActive = !effect.Logic.ToggleActive;
                        break;

                    case EffectType.PROC:
                        effect.DurationClock(true);
                        break;
                }
                return true;
            }
        }
        return false;
    }
    void GenerateRisidualEffect(Character target, PsystemType projectile = PsystemType.NONE)
    {
        Debug.Log("Generating risidual!");

        

        RootOptions rootOptions = new RootOptions(RootLogic.Options.GameState, this, ref RootLogic.Options.GameState.EFFECT_INDEX, target.Slots.Risiduals);
        //EffectOptions effectOptions = new EffectOptions(type, true, true);
        BaseEffect newEffect = GenerateEffect(rootOptions);

        newEffect.RootLogic.Options.Source = this;

        newEffect.ProjectileClock(true, projectile);
        newEffect.DurationClock(true);
        newEffect.PeriodClock(true);

        RootLogic.Clones.Add(newEffect);

        //UI_Options options = new UI_Options(newEffect, target, PlaceHolderType.EFFECT);
        target.Slots.Risiduals.Add(newEffect);
    }
    public virtual void RemoveRisidualEffect()
    {
        Destroy(this);
        /*if (EffectedCharacter == null)
            return;

        EffectedCharacter.Risiduals.Remove(this);

        EffectedCharacter = null;*/
    }
    public virtual void Amplify(float amp)
    {

    }
    public virtual void InitializeEffect(GameState state, EffectOptions options)
    {
        Debug.Log("Initializing Effect...");
        base.InitializeRoot(state);
        Logic.Options = options;
    }
    /*public override void Clone(RootScriptObject source, RootOptions options)
    {
        base.Clone(source, options);
    }*/
    /*public virtual void CloneEffect(BaseEffect source)
    {
        
    }*/
    public override void Clone(RootOptions options)
    {
        base.Clone(options);
        if (!(options.Source is BaseEffect))
            return;

        BaseEffect effect = (BaseEffect)options.Source;

        ProjectileType = effect.ProjectileType;
        Target = effect.Target;
        AOE_Range = effect.AOE_Range;

        ProjectileDuration = effect.ProjectileDuration;
        PeriodDuration = effect.PeriodDuration;
        RisidualDuration = effect.RisidualDuration;

        Logic.Clone(effect.Logic);
    }
    public BaseEffect GenerateEffect(RootOptions rootOptions, bool projectile = false)
    {
        Debug.Log("Generating effect...");
        BaseEffect newEffect = (BaseEffect)GenerateRootObject(rootOptions);
        newEffect.Clone(rootOptions);
        newEffect.Logic.IsProjectile = projectile;
        Debug.Log("Effect generated!");
        return newEffect;
    }
}
