using System.Collections.Generic;
using UnityEngine;

public enum EffectType
{
    PASSIVE,
    PROC,
    TOGGLE
}

//[CreateAssetMenu(fileName = "Effect", menuName = "ScriptableObjects/Effects")]
public class BaseEffect : ScriptableObject
{
    [Header("Base Properties")]
    public string Name;
    public Sprite Sprite;

    public GameObject Condition;
    public GameObject Projectile;
    public GameObject Impact;

    public PsystemType ProjectileType;

    public float PeriodLength;
    public float DurationLength;
    public float ProjectileLength;

    [Header("LOGIC - Do Not Touch!")]
    public EffectType EffectType;
    public float TetherRange;
    //public int EffectIndex;
    public bool Toggle;

    public float PeriodTimer;
    public float DurationTimer;
    public float ProjectileTimer;

    public CharacterAbility Source;
    public Character EffectedCharacter;
    public List<BaseEffect> Clones;

    public bool PeriodClock(bool reset = false)
    {
        if (reset)
        {
            PeriodTimer = 0;
            return true;
        }

        if (PeriodTimer != 0)
            PeriodTimer -= GlobalConstants.TIME_SCALE;

        if (PeriodTimer < 0)
            PeriodTimer = 0;

        return PeriodTimer == 0;
    }
    public bool DurationClock(bool reset = false)
    {
        if (EffectType != EffectType.PROC)
            return false;

        if (reset)
        {
            DurationTimer = DurationLength;
            return true;
        }

        if (DurationTimer != 0)
            DurationTimer -= GlobalConstants.TIME_SCALE;

        if (DurationTimer < 0)
            DurationTimer = 0;

        return DurationTimer == 0;
    }
    public bool ProjectileClock(bool reset = false)
    {
        if (reset)
        {
            ProjectileTimer = ProjectileLength;
            return true;
        }

        if (ProjectileTimer != 0)
            ProjectileTimer -= GlobalConstants.TIME_SCALE;

        if (ProjectileTimer < 0)
            ProjectileTimer = 0;

        return ProjectileTimer == 0;
    }
    void ProjectilePsystem()
    {
        Debug.Log("Stepped into Cast system");

        if (Source == null ||
            Source.SourceCharacter == null)
            return;

        Debug.Log("Casting...");

        if (Projectile != null)
            Destroy(Projectile);

        Projectile = Instantiate(Source.SourceCharacter.GameState.SceneMan.PsystemPrefabs[(int)ProjectileType], Source.SourceCharacter.transform);
        Projectile.transform.localPosition = Vector3.zero;
        ProjectileTimer = ProjectileLength;
    }
    public virtual void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        if (cast)
        {
            Toggle = toggle;
            if (!Toggle)
                return;

            if (ProjectileLength > 0 ||
                DurationLength > 0 ||
                EffectType == EffectType.PASSIVE ||
                EffectType == EffectType.TOGGLE)
            {
                GenerateRisidualEffect(target);
            }
        }

        if (!cast)
        {
            if (!ProjectileClock())
                return;

            if (Projectile != null)
                Destroy(Projectile);

            if (DurationClock() &&
                EffectType == EffectType.PROC)
                RemoveRisidualEffect();
        }
    }
    /*void EngageRisidual(Character target, bool toggle = true) // Post Projectile
    {
        switch (EffectType)
        {
            case EffectType.PROC:
            case EffectType.PASSIVE:
                GenerateRisidualEffect(target);
                break;

            case EffectType.TOGGLE:
                if (toggle)
                    GenerateRisidualEffect(target);
                else
                    RemoveRisidualEffect();
                break;
        }
    }*/
    public void GenerateRisidualEffect(Character target)
    {
        foreach (BaseEffect effect in target.Risiduals)
        {
            if (Clones.Find(x => x == effect) != null)
            {
                switch(EffectType)
                {
                    case EffectType.TOGGLE:
                        effect.Toggle = !effect.Toggle;
                        break;

                    case EffectType.PROC:
                        effect.DurationClock(true);
                        break;
                }
                return;
            }
        }

        BaseEffect newEffect = GenerateEffect(target, false);

        newEffect.ProjectileClock(true);
        newEffect.DurationClock(true);
        newEffect.PeriodClock(true);

        Clones.Add(newEffect);

        switch (EffectType)
        {
            case EffectType.PASSIVE:
            case EffectType.TOGGLE:
                target.Risiduals.Add(newEffect);
                break;

            case EffectType.PROC:
                if (DurationLength > 0) // Timed
                    target.Risiduals.Add(newEffect);
                break;
        }
    }
    public virtual void RemoveRisidualEffect()
    {
        if (EffectedCharacter == null)
            return;

        EffectedCharacter.Risiduals.Remove(this);
        EffectedCharacter = null;
    }

    public virtual void Amplify(float amp)
    {

    }
    public virtual void InitializeSource()
    {

    }
    public virtual void CloneEffect(BaseEffect source, bool inject = false)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Condition = source.Condition;
        Impact = source.Impact;
        EffectType = source.EffectType;
        TetherRange = source.TetherRange;
        Source = source.Source;
        DurationLength = source.DurationLength;
        DurationTimer = DurationLength;
        ProjectileLength = source.ProjectileLength;
    }
    public virtual BaseEffect GenerateEffect(Character effected = null, bool inject = true)
    {
        BaseEffect newEffect = (BaseEffect)CreateInstance("BaseEffect");
        newEffect.CloneEffect(this, inject);
        newEffect.EffectedCharacter = effected;
        return newEffect;
    }
}
