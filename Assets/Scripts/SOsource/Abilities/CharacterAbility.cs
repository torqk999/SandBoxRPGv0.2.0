using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CastLocation
{
    WEP_TIP,
    WEP_MID,
    HAND,
    SELF
}
public enum TargetType
{
    ALL,
    SELF,
    ALLY,
    ENEMY
}

//[CreateAssetMenu(fileName = "CharacterAbility", menuName = "ScriptableObjects/CharacterAbility")]
public class CharacterAbility : ScriptableObject
{
    [Header("Ability Properties")]
    public string Name;
    public Sprite Sprite;
    public ParticleSystem Cast;
    public CastLocation CastLocation;

    public CharAnimationState AnimationState;
    public CharAnimation CharAnimation;

    public float CostValue;
    public RawStat CostTarget;
    public ValueType CostType;

    public School School;
    public float CastRange;

    public float CD_Duration;
    public float CD_Timer;
    public List<BaseEffect> SpawnedEffects;

    public virtual void UseAbility(Character target) { }
    public virtual void Amplify(CharacterSheet sheet = null, Equipment equip = null) { }

    public virtual void CloneAbility(CharacterAbility source, bool inject = false)
    {
        Name = source.Name;
        Sprite = source.Sprite;

        AnimationState = source.AnimationState;
        CharAnimation = source.CharAnimation;

        CostValue = source.CostValue;
        CostTarget = source.CostTarget;
        CostType = source.CostType;

        School = source.School;
        CastRange = source.CastRange;

        CD_Duration = source.CD_Duration;
        CD_Timer = 0;
        SpawnedEffects = new List<BaseEffect>();
    }
    public void SetCooldown()
    {
        CD_Timer = CD_Duration;
    }
    public void UpdateCooldown()
    {
        CD_Timer -= GlobalConstants.TIME_SCALE;
        CD_Timer = (CD_Timer < 0) ? 0 : CD_Timer;
    }
    public virtual CharacterAbility GenerateAbility(bool inject = true)
    {
        CharacterAbility newAbility = (CharacterAbility)CreateInstance("CharacterAbility");
        newAbility.CloneAbility(this, inject);
        return newAbility;
    }
    public virtual CharacterAbility EquipAbility(Character currentCharacter, Equipment equip)
    {
        CharacterAbility newAbility = GenerateAbility();
        newAbility.Amplify(currentCharacter.Sheet, equip);
        currentCharacter.Abilities.Add(newAbility);
        return newAbility;
    }
}
