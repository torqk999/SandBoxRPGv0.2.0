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

    public School School;
    public ValueType CostType;
    public RawStat CostTarget;

    public float CostValue;
    public float CastRange;
    public float AOE_Range;
    public float CD_Duration;

    [Header("Ability Logic - Do Not Touch")]
    public int AbilityID;
    public int EquipID;
    public float CD_Timer;
    public List<BaseEffect> SpawnedEffects;
    public Character Source;

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
        AOE_Range = source.AOE_Range;
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
    public virtual CharacterAbility EquipAbility(Character currentCharacter, int abilityID, Equipment equip = null)
    {
        CharacterAbility newAbility = GenerateAbility();
        newAbility.Amplify(currentCharacter.Sheet, equip);
        newAbility.Source = currentCharacter;
        newAbility.AbilityID = abilityID;
        newAbility.EquipID = equip == null ? -1 : equip.EquipID;
        currentCharacter.Abilities.Add(newAbility);
        return newAbility;
    }
}
