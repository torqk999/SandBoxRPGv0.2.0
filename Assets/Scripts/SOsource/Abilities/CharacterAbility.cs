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
    public GameObject Cast;
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
    

    [Header("Ability Logic - Do Not Touch")]
    //public int AbilityID;
    //public int EquipID;
    public float CD_Timer;
    public float Cast_Timer;
    public float Projectile_Timer;
    public List<BaseEffect> Projectiles;
    public Character SourceCharacter;

    public virtual void UseAbility(Character target)
    {
        CastPsystem();
    }
    void CastPsystem()
    {
        Debug.Log("Stepped into Cast system");

        if (SourceCharacter == null)
            return;

        Debug.Log("Casting...");

        if (Cast != null)
            Destroy(Cast);

        Cast = Instantiate(SourceCharacter.GameState.SceneMan.PsystemPrefabs[(int)CastType], SourceCharacter.transform);
        Cast.transform.localPosition = Vector3.zero;
        Cast_Timer = Cast_Duration;
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
        

        CD_Timer = 0;
        Cast_Timer = 0;
        Projectile_Timer = 0;

        Projectiles = new List<BaseEffect>();
    }
    public void SetCooldown()
    {
        CD_Timer = CD_Duration;
    }
    public void UpdateCooldowns()
    {
        if (CD_Timer != 0)
        {
            CD_Timer -= GlobalConstants.TIME_SCALE;
            CD_Timer = (CD_Timer < 0) ? 0 : CD_Timer;
        }
        if (Cast_Timer != 0)
        {
            Cast_Timer -= GlobalConstants.TIME_SCALE;
            Cast_Timer = (Cast_Timer < 0) ? 0 : Cast_Timer;
        }
        if (Cast_Timer == 0 && Cast != null)
        {
            Destroy(Cast);
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
        SourceCharacter = currentCharacter;

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
