using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CharacterRender;

/*public enum CostType
{
    HEALTH,
    STAMINA,
    MANA,
    NONE
}*/
public enum RangeType
{
    SELF,
    TARGET,
    TARGET_AOE,
    AOE,
    SHAPED_VOLUME,
    SUMMON
}
public enum AbilityType
{
    ATTACK,
    SPELL,
    POWER
}
[CreateAssetMenu(fileName = "CharacterSheet", menuName = "ScriptableObjects/CharacterAbility")]
public class CharacterAbility : ScriptableObject
{
    public int EquipID;
    public string Name;
    public Sprite Sprite;
    public bool bTargetEnemy;
    public bool bIsPassive;
    public RangeType RangeType;
    public RawStat CostTarget;
    public ValueType CostType;
    public AbilityType AbilityType;
    public CharAnimationState AnimationState;
    public CharAnimation CharAnimation;
    public AnimationTarget AnimationTarget;
    public float RangeValue;
    public float CostValue;
    public float CD_Duration;
    public float CD_Timer;
    public Effect[] Effects;

    public void Clone(CharacterAbility ability, int equipId = -1, float potency = 1, bool inject = false)
    {
        EquipID = equipId;// ability.WeaponID; // <<<--- ?!?!?
        Name = ability.Name;
        Sprite = ability.Sprite;
        RangeType = ability.RangeType;
        CostTarget = ability.CostTarget;
        AbilityType = ability.AbilityType;
        RangeValue = ability.RangeValue;
        CostValue = ability.CostValue;
        CD_Duration = ability.CD_Duration;
        CD_Timer = 0;
        Effects = new Effect[ability.Effects.Length];
        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = (Effect)ScriptableObject.CreateInstance("Effect");
            Effects[i].Clone(ability.Effects[i], equipId, potency, inject);
            //Effects[i].ElementPack.Amplify(potency);
        }
    }
    public CharacterAbility EquipAbility(Character currentCharacter, Equipment equip)
    {
        
        //int[] characterSkillLevels = currentCharacter.Sheet.Level.PullData();

        float potency = 1 +

            (((currentCharacter.Sheet.Skills.Levels[(int)equip.EquipSkill] * CharacterMath.CHAR_LEVEL_FACTOR) +                      // Level

            (equip.EquipLevel * CharacterMath.WEP_LEVEL_FACTOR) +                                                                    // Weapon

            (currentCharacter.Sheet.Skills.Levels[(int)equip.EquipSkill] * CharacterMath.SKILL_MUL_LEVEL[(int)equip.EquipSkill])) *  // Skill

            CharacterMath.SKILL_MUL_RACE[(int)currentCharacter.Sheet.Race, (int)equip.EquipSkill]);                                  // Race

        CharacterAbility newAbility = (CharacterAbility)ScriptableObject.CreateInstance("CharacterAbility");
        newAbility.Clone(this, equip.EquipID, potency);
        return newAbility;
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


    // Add animation/projectile prefabs & assets here
}
