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
            return Logic.SourceAbility.Logic.SourceCharacter;
        }
        catch
        {
            Debug.Log($"Bad SourceCharacter reference on effect: {Name}");
            return null;
        }
    }
    public bool HasEligableTarget()
    {
        if (RootLogic.GameState == null)
            return false;

        if (SourceCharacter() == null)
            return false;

        if (Target == TargetType.SELF)
            return true;

        if (AOE_Range <= 0)
            return false;

        if (RootLogic.GameState.CharacterMan.CharacterPool == null ||
            RootLogic.GameState.CharacterMan.CharacterPool.Count < 1)
            return false;

        List<Character> pool = RootLogic.GameState.CharacterMan.CharacterPool;

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

        if (Logic.SourceAbility == null ||
            Logic.SourceAbility.Logic.SourceCharacter == null ||
            Logic.EffectedCharacter == null)
            return;

        Debug.Log("Firing...");

        if (lerp == 0)
        {
            Destroy(Logic.Projectile);
            return;
        }
        if (Logic.Projectile == null)
        {
            if (RootLogic.GameState == null)
                return;

            Logic.Projectile = Instantiate(RootLogic.GameState.SceneMan.PsystemPrefabs[(int)ProjectileType], Logic.SourceAbility.Logic.SourceCharacter.transform);
            Logic.Projectile.transform.localPosition = Vector3.zero;
        }
        //Projectile.transform.rotation = Quaternion.Lerp(EffectedCharacter.Root.rotation, SourceAbility.SourceCharacter.Root.rotation, lerp);
        Logic.Projectile.transform.LookAt(Logic.EffectedCharacter.Root);
        Logic.Projectile.transform.position = Vector3.Lerp(Logic.EffectedCharacter.Root.position, Logic.SourceAbility.Logic.SourceCharacter.Root.position, lerp);
    }
    public virtual void ApplySingleEffect(Character target, EffectOptions options, bool cast = false)
    {
        if (cast)
        {
            Logic.Options.ToggleActive = options.ToggleActive;

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

            if (Logic.Projectile != null)
                Destroy(Logic.Projectile);

            if (DurationClock() &&
                Logic.Options.EffectType == EffectType.PROC)
                RemoveRisidualEffect();
        }
    }
    public void GenerateRisidualEffect(Character target, PsystemType projectile = PsystemType.NONE)
    {
        Debug.Log("Generating risidual!");

        foreach (BaseEffect effect in target.Risiduals)
        {
            if (RootLogic.Clones.Find(x => x == effect) != null)
            {
                switch(Logic.Options.EffectType)
                {
                    case EffectType.TOGGLE:
                        effect.Logic.Options.ToggleActive = !effect.Logic.Options.ToggleActive;
                        break;

                    case EffectType.PROC:
                        effect.DurationClock(true);
                        break;
                }
                return;
            }
        }

        EffectType type = default;
        if (Logic.SourceAbility != null)
            switch (Logic.SourceAbility)
            {
                case PassiveAbility:
                    type = EffectType.PASSIVE;
                    break;

                case ProcAbility:
                    type = EffectType.PROC;
                    break;

                case ToggleAbility:
                    type = EffectType.TOGGLE;
                    break;
            }

        RootOptions rootOptions = new RootOptions(this);
        EffectOptions effectOptions = new EffectOptions(type, true, true);
        BaseEffect newEffect = GenerateEffect(rootOptions, effectOptions ,target);

        newEffect.RootLogic.Original = this;

        newEffect.ProjectileClock(true, projectile);
        newEffect.DurationClock(true);
        newEffect.PeriodClock(true);

        RootLogic.Clones.Add(newEffect);

        target.Risiduals.Add(newEffect);
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
    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
    }
    /*public override void Clone(RootScriptObject source, RootOptions options)
    {
        base.Clone(source, options);
    }*/
    public virtual void CloneEffect(BaseEffect source, EffectOptions effectOptions, Character effected = null)
    {
        RisidualDuration = source.RisidualDuration;
        PeriodDuration = source.PeriodDuration;

        Logic.EffectedCharacter = effected;
        //Logic.Original = effectOptions.IsClone ? this : null;

        Logic.Clone(source.Logic, effectOptions);
    }
    /*public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == string.Empty ? "BaseEffect" : options.ClassID;
        return (BaseEffect)base.GenerateRootObject(options);
    }*/
    public virtual BaseEffect GenerateEffect(RootOptions rootOptions, EffectOptions effectOptions, Character effected = null)
    {
        //rootOptions.
        BaseEffect newEffect = (BaseEffect)GenerateRootObject(rootOptions);
        newEffect.Copy(this, rootOptions);
        Debug.Log("Effect generated!");
        return newEffect;
    }
}
