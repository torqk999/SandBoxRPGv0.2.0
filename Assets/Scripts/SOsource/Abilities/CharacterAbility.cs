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

public enum CastTarget
{
    CHAR,
    MOUSE,
    RANDOM
}

//[CreateAssetMenu(fileName = "CharacterAbility", menuName = "ScriptableObjects/CharacterAbility")]
public class CharacterAbility : RootScriptObject
{
    [Header("Ability Properties")]
    public CharAnimationState AnimationState;
    public CharAnimation CharAnimation;

    public PsystemType CastLocationType;
    public CastLocation CastLocation;

    public PsystemType CastTargetType;
    public CastTarget CastTarget;
    public TargetType TargetType;

    public School School; // CC prevention for now, damage scaling later
    public ValueType CostType;
    public RawStat CostTarget;

    public float CostValue;
    public float CD_Duration;
    public float Cast_Duration;
    public float CastRange;

    public AbilityLogic Logic;

    public virtual bool HasEligableTarget()
    {
        switch (CastTarget)
        {
            case CastTarget.CHAR:
                if (TargetType == TargetType.SELF)
                    return true;
                if (CastRange <= 0 &&
                    Logic.SourceCharacter.CurrentTargetCharacter != Logic.SourceCharacter) // awkward corner case is awkward
                {
                    Debug.Log("Ability has no range to target ");
                    return false;
                }

                if (Vector3.Distance(Logic.SourceCharacter.Root.position, Logic.SourceCharacter.CurrentTargetCharacter.Root.position) > CastRange)
                    return false; // Range Check
                if (TargetType == TargetType.ALLY)
                        return Logic.SourceCharacter.CheckAllegiance(Logic.SourceCharacter.CurrentTargetCharacter);
                if (TargetType == TargetType.ENEMY)
                    return !Logic.SourceCharacter.CheckAllegiance(Logic.SourceCharacter.CurrentTargetCharacter);
                return true; // All check

            case CastTarget.MOUSE:
                return false;

            case CastTarget.RANDOM:
                return false;

        }
        return false;
    }
    public virtual void UseAbility(Character target, EffectOptions options)
    {
        CastPsystem();
    }
    public virtual void EquipAbility(Character currentCharacter, Equipment equip = null)
    {
        Amplify(currentCharacter.Sheet, equip);
        currentCharacter.Slots.Skills.List.Add(this);
        Logic.SourceCharacter = currentCharacter;
    }
    void CastPsystem()
    {
        Debug.Log("Stepped into Cast system");

        if (CastLocationType == PsystemType.NONE)
            return;

        if (Logic.SourceCharacter == null)
            return;

        Debug.Log("Casting...");

        if (Logic.CastLocationInstance != null)
            Destroy(Logic.CastLocationInstance);

        Logic.CastLocationInstance = Instantiate(Logic.SourceCharacter.GameState.SceneMan.PsystemPrefabs[(int)CastLocationType], Logic.SourceCharacter.transform);
        Logic.CastLocationInstance.transform.localPosition = Vector3.zero;
        Logic.Cast_Timer = Cast_Duration;
    }
    public virtual void Amplify(CharacterSheet sheet = null, Equipment equip = null)
    {

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
        if (Logic.Cast_Timer == 0 && Logic.CastLocationInstance != null)
        {
            Destroy(Logic.CastLocationInstance);
        }
    }
    public void UpdateProjectiles()
    {

    }
    public virtual void UpdatePassiveTimer()
    {

    }
    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
    }
    /*public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = "CharacterAbility";
        CharacterAbility newRootObject = (CharacterAbility)CreateInstance(options.ClassID);
        newRootObject.Clone(this, options);
        return newRootObject;
    }*/
    public virtual CharacterAbility GenerateAbility(RootOptions options)
    {
        //options.Root = options.Root == "" ? "CharacterAbility" : options.Root;
        CharacterAbility newAbility = (CharacterAbility)GenerateRootObject(options);
        newAbility.Copy(this, options);
        return newAbility;
    }
    public override void Copy(RootScriptObject source, RootOptions options = default)
    {
        base.Copy(source, options);

        if (!(source is CharacterAbility))
            return;
        CharacterAbility ability = (CharacterAbility)source;

        AnimationState = ability.AnimationState;
        CharAnimation = ability.CharAnimation;

        CostValue = ability.CostValue;
        CostTarget = ability.CostTarget;
        CostType = ability.CostType;

        School = ability.School;

        CD_Duration = ability.CD_Duration;
        Cast_Duration = ability.Cast_Duration;

        Logic.Clone(ability.Logic);

        Logic.CD_Timer = 0;
        Logic.Cast_Timer = 0;
    }
    
}
