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
    public ParticleSystem Condition;
    public ParticleSystem Impact;

    [Header("Persistant Properties - Do Not Touch")]
    public CharacterSheet SourceSheet;
    public CharacterAbility SourceAbility;
    public Equipment SourceEquip;
    public List<BaseEffect> Residing;

    public EffectType EffectType;

    public float DurationLength;
    public float DurationTimer;
    public float PeriodLength;
    public float PeriodTimer;

    public virtual void ApplySingleEffect(Character target, bool cast = false, bool toggle = true)
    {
        if (!cast)
            return;
        switch(EffectType)
        {
            case EffectType.PASSIVE:
                AddRisidualEffect(target);
                break;

            case EffectType.TOGGLE:
                if (toggle)
                    AddRisidualEffect(target);
                else
                    RemoveRisidualEffect(target);
                break;

            case EffectType.PROC:
                if (DurationLength > 0)
                    AddRisidualEffect(target);
                break;
        }  
    }
    public void AddRisidualEffect(Character target)
    {
        switch(EffectType)
        {
            case EffectType.PASSIVE:
                BaseEffect newPassive = GenerateEffect(false);
                newPassive.Amplify();
                target.Risiduals.Add(newPassive);
                break;

            case EffectType.TOGGLE:
                target.Risiduals.Add(GenerateEffect(false));
                break;

            case EffectType.PROC:
                if (DurationLength > 0) // Timed
                    target.Risiduals.Add(GenerateEffect(false));
                break;
        }
    }
    public virtual void RemoveRisidualEffect(Character character)
    {

    }
    public virtual void Amplify(float amp)
    {

    }
    public virtual void CloneEffect(BaseEffect source, bool inject = false)
    {
        Name = source.Name;
        Sprite = source.Sprite;
        Condition = source.Condition;
        Impact = source.Impact;
        EffectType = source.EffectType;

        //SourceSheet = sheet;
        //SourceAbility = ability;
        //SourceEquip = equip;


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
