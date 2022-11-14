using UnityEngine;

public enum EffectType
{
    PASSIVE,
    PROC,
    TOGGLE
}

public class BaseEffect : ScriptableObject
{
    [Header("Base Properties")]
    public string Name;
    public Sprite Sprite;

    public PsystemType ProjectileType;

    public float PeriodLength;
    public float DurationLength;

    [Header("NO TOUCHY!")]
    public EffectLogic Logic;

    public GameState GameState() // Because I am le lazy...
    {
        try
        {
            return Logic.SourceAbility.Logic.SourceCharacter.GameState;
        }
        catch
        {
            Debug.Log($"Bad GameState reference on effect: {Name}");
            return null;
        }
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
            Logic.DurationTimer = DurationLength;
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
            Logic.ProjectileTimer = Logic.ProjectileLength;
            return true;
        }

        if (Logic.ProjectileTimer != 0)
            Logic.ProjectileTimer -= GlobalConstants.TIME_SCALE;

        if (Logic.ProjectileTimer < 0)
            Logic.ProjectileTimer = 0;

        ProjectilePsystem(Logic.ProjectileTimer / Logic.ProjectileLength, projectile);

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
            if (GameState() == null)
                return;
            Logic.Projectile = Instantiate(GameState().SceneMan.PsystemPrefabs[(int)ProjectileType], Logic.SourceAbility.Logic.SourceCharacter.transform);
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

            if (Logic.ProjectileLength > 0 ||
                DurationLength > 0 ||
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
            if (Logic.Clones.Find(x => x == effect) != null)
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

        EffectOptions options = new EffectOptions();
        BaseEffect newEffect = GenerateEffect(options ,target);

        newEffect.Logic.Original = this;

        newEffect.ProjectileClock(true, projectile);
        newEffect.DurationClock(true);
        newEffect.PeriodClock(true);

        Logic.Clones.Add(newEffect);

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
    public virtual void InitializeSource()
    {

    }
    public virtual void CloneEffect(BaseEffect source, EffectOptions options)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        DurationLength = source.DurationLength;
        PeriodLength = source.PeriodLength;

        Logic.Clone(source.Logic, options);
    }
    public virtual BaseEffect GenerateEffect(EffectOptions options, Character effected = null)
    {
        BaseEffect newEffect = (BaseEffect)CreateInstance("BaseEffect");
        newEffect.CloneEffect(this, options);
        
        newEffect.Logic.EffectedCharacter = effected;
        newEffect.Logic.Original = options.IsClone ? this : null;

        return newEffect;
    }
}
