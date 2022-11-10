using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "CharacterAbility", menuName = "ScriptableObjects/CharacterAbility")]
public class CharacterAbility : ScriptableObject
{
    [Header("Ability Properties")]
    public int EquipID;
    public string Name;
    public Sprite Sprite;

    public CharAnimationState AnimationState;
    public CharAnimation CharAnimation;
    public AnimationTarget AnimationTarget;

    public RawStat CostTarget;
    public ValueType CostType;
    public AbilitySchool School;

    public float RangeValue;
    public float CostValue;
    public float CD_Duration;
    public float CD_Timer;

    public void CloneAbility(CharacterAbility ability, int equipId = -1, float potency = 1, bool inject = false)
    {
        EquipID = equipId;
        Name = ability.Name;
        Sprite = ability.Sprite;

        AnimationState = ability.AnimationState;
        CharAnimation = ability.CharAnimation;
        AnimationTarget = ability.AnimationTarget;

        CostTarget = ability.CostTarget;
        CostType = ability.CostType;
        School = ability.School;

        RangeValue = ability.RangeValue;
        CostValue = ability.CostValue;
        CD_Duration = ability.CD_Duration;
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

    (((currentCharacter.Sheet.Skills.Levels[(int)equip.EquipSkill] * CharacterMath.CHAR_LEVEL_FACTOR) +                      // Level

    (equip.EquipLevel * CharacterMath.WEP_LEVEL_FACTOR) +                                                                    // Weapon

    (currentCharacter.Sheet.Skills.Levels[(int)equip.EquipSkill] * CharacterMath.SKILL_MUL_LEVEL[(int)equip.EquipSkill])) *  // Skill

    CharacterMath.SKILL_MUL_RACE[(int)currentCharacter.Sheet.Race, (int)equip.EquipSkill]);                                  // Race
    }

    virtual public CharacterAbility EquipAbility(Character currentCharacter, Equipment equip, bool inject)
    {
        CharacterAbility newAbility = (CharacterAbility)CreateInstance("CharacterAbility");
        newAbility.CloneAbility(this, equip.EquipID);
        return newAbility;
    }
}
