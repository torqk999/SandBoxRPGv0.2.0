using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetType
{
    ALL,
    SELF,
    ALLY,
    ENEMY
}
public enum AbilitySchool
{
    ATTACK,
    SPELL,
    POWER
}
//[CreateAssetMenu(fileName = "CharacterAbility", menuName = "ScriptableObjects/CharacterAbility")]
public class CharacterAbility : ScriptableObject
{
    [Header("Ability Properties")]
    public int EquipID;
    public string Name;
    public Sprite Sprite;

    public CharAnimationState AnimationState;
    public CharAnimation CharAnimation;

    public float CostValue;
    public RawStat CostTarget;
    public ValueType CostType;

    public AbilitySchool School;
    public float CastRange;
    public float CD_Duration;
    public float CD_Timer;

    public virtual void CloneAbility(CharacterAbility source, int equipId, float potency = 1, bool inject = false)
    {
        EquipID = equipId;
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
    public float GeneratePotency(Character currentCharacter, Equipment equip)
    {
        /*float potency =*/ return 1 +

    (((currentCharacter.Sheet.SkillsLevels.Levels[(int)equip.EquipSkill] * CharacterMath.CHAR_LEVEL_FACTOR) +                      // Level

    (equip.EquipLevel * CharacterMath.WEP_LEVEL_FACTOR) +                                                                    // Weapon

    (currentCharacter.Sheet.SkillsLevels.Levels[(int)equip.EquipSkill] * CharacterMath.SKILL_MUL_LEVEL[(int)equip.EquipSkill])) *  // Skill

    CharacterMath.SKILL_MUL_RACE[(int)currentCharacter.Sheet.Race, (int)equip.EquipSkill]);                                  // Race
    }

    virtual public CharacterAbility EquipAbility(Character currentCharacter, Equipment equip, bool inject)
    {
        CharacterAbility newAbility = (CharacterAbility)CreateInstance("CharacterAbility");
        newAbility.CloneAbility(this, equip.EquipID);
        return newAbility;
    }
}
