using System.Text;
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
    
    public PsystemType CastType;
    public CastLocation CastLocation;

    public CharAnimationState AnimationState;
    public CharAnimation CharAnimation;

    public School School; // CC prevention for now, damage scaling later
    public ValueType CostType;
    public RawStat CostTarget;

    public float CostValue;
    public float CastRange;
    public float AOE_Range;
    public float CD_Duration;
    public float Cast_Duration;
    public float ProjectileLength;

    public AbilityLogic Logic;

    public virtual void UseAbility(Character target, EffectOptions optiions)
    {
        CastPsystem();
    }
    void CastPsystem()
    {
        Debug.Log("Stepped into Cast system");

        if (CastType == PsystemType.NONE)
            return;

        if (Logic.SourceCharacter == null)
            return;

        Debug.Log("Casting...");

        if (Logic.CastInstance != null)
            Destroy(Logic.CastInstance);

        Logic.CastInstance = Instantiate(Logic.SourceCharacter.GameState.SceneMan.PsystemPrefabs[(int)CastType], Logic.SourceCharacter.transform);
        Logic.CastInstance.transform.localPosition = Vector3.zero;
        Logic.Cast_Timer = Cast_Duration;
    }
    public virtual void Amplify(CharacterSheet sheet = null, Equipment equip = null)
    {

    }
    public virtual void InitializeSource()
    {

    }
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
        Cast_Duration = source.Cast_Duration;
        ProjectileLength = source.ProjectileLength;

        Logic.Clone(source.Logic);

        Logic.CD_Timer = 0;
        Logic.Cast_Timer = 0;
    }
    public void SetCooldown()
    {
        Logic.CD_Timer = CD_Duration;
    }
    public void UpdateCooldowns()
    {
        if (Logic.CD_Timer != 0)
        {
            Logic.CD_Timer -= GlobalConstants.TIME_SCALE;
            Logic.CD_Timer = (Logic.CD_Timer < 0) ? 0 : Logic.CD_Timer;
        }
        if (Logic.Cast_Timer != 0)
        {
            Logic.Cast_Timer -= GlobalConstants.TIME_SCALE;
            Logic.Cast_Timer = (Logic.Cast_Timer < 0) ? 0 : Logic.Cast_Timer;
        }
        if (Logic.Cast_Timer == 0 && Logic.CastInstance != null)
        {
            Destroy(Logic.CastInstance);
        }
    }
    public void UpdateProjectiles()
    {

    }
    public virtual void UpdatePassiveTimer()
    {

    }
    public virtual CharacterAbility GenerateAbility(bool inject = true)
    {
        CharacterAbility newAbility = (CharacterAbility)CreateInstance("CharacterAbility");
        newAbility.CloneAbility(this, inject);
        newAbility.InitializeSource();
        return newAbility;
    }
    public virtual void EquipAbility(Character currentCharacter, Equipment equip = null)
    {
        Amplify(currentCharacter.Sheet, equip);
        currentCharacter.Abilities.Add(this);
        //Debug.Log(this.GetType().ToString());
        Logic.SourceCharacter = currentCharacter;

        /*
        CharacterAbility newAbility = GenerateAbility();
        newAbility.Amplify(currentCharacter.Sheet, equip);
        newAbility.Source = currentCharacter;
        newAbility.AbilityID = abilityID;
        newAbility.EquipID = equip == null ? -1 : equip.EquipID;
        currentCharacter.Abilities.Add(newAbility);
        return newAbility;
        */
    }
}
