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

    public float PeriodLength;
    public float DurationLength;
    public float ProjectileLength;

    [Header("LOGIC - Do Not Touch!")]
    public EffectType EffectType;
    public float TetherRange;
    public int EffectIndex;

    public float PeriodTimer;
    public float DurationTimer;
    public float ProjectileTimer;

    public CharacterAbility Source;
    public Character EffectedCharacter;

    public bool PeriodClock(bool reset = false)
    {
        if (reset)
        {
            PeriodTimer = 0;
            return true;
        }

        PeriodTimer -= GlobalConstants.TIME_SCALE;
        if (PeriodTimer <= 0)
            PeriodTimer = PeriodLength;

        return PeriodTimer == PeriodLength;
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

        DurationTimer -= GlobalConstants.TIME_SCALE;
        if (DurationTimer <= 0)
            DurationTimer = DurationLength;

        return DurationTimer == DurationLength;
    }
    public virtual void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        if (!cast)
        {
            if (DurationClock())
                RemoveRisidualEffect();
            return;
        }
            
        switch(EffectType)
        {
            case EffectType.PROC:
            case EffectType.PASSIVE:
                AddRisidualEffect(target);
                break;

            case EffectType.TOGGLE:
                if (toggle)
                    AddRisidualEffect(target);
                else
                    RemoveRisidualEffect();
                break;
        }  
    }
    public void AddRisidualEffect(Character target)
    {
        foreach (BaseEffect effect in target.Risiduals)
            if (effect.Source == Source &&
                effect.EffectIndex == EffectIndex)
                Destroy(effect);

        switch(EffectType)
        {
            case EffectType.PASSIVE:
            case EffectType.TOGGLE:
                target.Risiduals.Add(GenerateEffect(false));
                break;

            case EffectType.PROC:
                if (DurationLength > 0) // Timed
                    target.Risiduals.Add(GenerateEffect(false));
                break;
        }

        EffectedCharacter = target;
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
    }
    public virtual BaseEffect GenerateEffect(bool inject = true)
    {
        BaseEffect newEffect = (BaseEffect)CreateInstance("BaseEffect");
        newEffect.CloneEffect(this, inject);
        return newEffect;
    }
}
