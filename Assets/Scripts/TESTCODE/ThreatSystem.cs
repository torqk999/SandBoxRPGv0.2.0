using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Examples:
Resistance
!RampUp = N -> 0
int(Target.Res[(int)Element] / RampValue)
1 - (TargetValue / RampValue) * ThreatValue
 */
public enum ThreatRelation
{
    SELF,
    TARGET,
    ALLY,
    FOE
}
public enum ThreatType
{
    Stat_Relative,
    Stat_Current,
    Res,
    CC,
    Immunity,
    Proximity
}
/*public enum ThreatRamp
{
    flat,
    percent
}*/
public interface IthreatSingleCondition
{
    int CurrentValue(Character character, Character source);
}
public interface IthreatSummativeCondition
{
    int CurrentValue(Character character, List<Character> source);
}
public class ThreatCondition
{
    public bool RampUp;
    public int RampValue;
    public int ThreatValue;

    //public ThreatType Type;
    //public ThreatRamp Ramp;
}
public class ResCondition : ThreatCondition, IthreatSingleCondition
{
    public Element Target;

    public int CurrentValue(Character character, Character source)
    {
        if (character == null)
            return 0;

        if (character.Resistances.Elements == null ||
            character.Resistances.Elements.Length != CharacterMath.STATS_ELEMENT_COUNT)
            return 0;

        float output = character.Resistances.Elements[(int)Target] / RampValue;
        output = RampUp ? output : 1 - output;
        return (int)(ThreatValue * output);
    }
}
public class StatCurrentCondition : ThreatCondition, IthreatSingleCondition
{
    public RawStat Target;

    public int CurrentValue(Character character, Character source)
    {
        if (character == null)
            return 0;

        if (character.CurrentStats.Stats == null || character.CurrentStats.Stats.Length != CharacterMath.STATS_RAW_COUNT ||
            character.MaximumStatValues.Stats == null || character.MaximumStatValues.Stats.Length != CharacterMath.STATS_RAW_COUNT)
            return 0;

        float output = character.CurrentStats.Stats[(int)Target] / character.MaximumStatValues.Stats[(int)Target];
        output = RampUp ? output : 1 - output;
        return (int)(ThreatValue * output);
    }
}
public class StatRelativeCondition : ThreatCondition, IthreatSingleCondition
{
    public RawStat Target;

    public int CurrentValue(Character character, Character source)
    {
        if (character == null ||
            source == null)
            return 0;

        if (character.CurrentStats.Stats == null || character.CurrentStats.Stats.Length != CharacterMath.STATS_RAW_COUNT ||
            source.CurrentStats.Stats == null || source.CurrentStats.Stats.Length != CharacterMath.STATS_RAW_COUNT)
            return 0;

        float output = character.CurrentStats.Stats[(int)Target] / source.CurrentStats.Stats[(int)Target];
        output = RampUp ? output : 1 - output;
        return (int)(ThreatValue * output);
    }
}
public class CCcondition : ThreatCondition, IthreatSingleCondition
{
    public CCstatus Target;

    public int CurrentValue(Character character, Character source)
    {
        if (character == null)
            return 0;

        return character.CheckCCstatus(Target) == RampUp ? ThreatValue : 0;
    }
}
public class ImmunityCondition : ThreatCondition, IthreatSingleCondition
{
    public int CurrentValue(Character character, Character source)
    {
        throw new System.NotImplementedException();
    }
}
public class ProxyCondition : ThreatCondition, IthreatSingleCondition, IthreatSummativeCondition
{
    public ThreatRelation Target;

    public int CurrentValue(Character character, Character source)
    {
        if (Target != ThreatRelation.SELF)
            return 0;

        float output = Vector3.Distance(character.Root.position, source.Root.position) / RampValue;
        output = RampUp ? output : 1 - output;
        return (int)(ThreatValue * output);
    }

    public int CurrentValue(Character character, List<Character> source)
    {
        if (Target == ThreatRelation.SELF)
            return 0;
        float output = 0;
        for (int i = 0; i < source.Count; i++)
        {
            float current = Vector3.Distance(character.Root.position, source[i].Root.position) / RampValue;
            current = RampUp ? current : 1 - current;
            output += (ThreatValue * current);
        }
        return (int)output;
    }
}
public class TargettedCondition : ThreatCondition, IthreatSingleCondition
{
    public ThreatRelation Target;

    public int CurrentValue(Character character, Character source)
    {
        throw new System.NotImplementedException();
    }
}
public struct ThreatAction
{
    public int Threshold;
    public CharacterAbility Ability;
}
public class ThreatProfile
{
    public int ThreatScore;
    public Character Character;

    public void UpdateScore(ref List<ThreatCondition> conditions)
    {

    }

    float PullTargetValue(ThreatType type)
    {
        switch (type)
        {
            case ThreatType.Res:
                break;
        }

        return -1;
    }
}

public class ThreatSystem : MonoBehaviour
{
    public List<ThreatCondition> Conditions = new List<ThreatCondition>();
    public List<ThreatProfile> Profiles = new List<ThreatProfile>();
    public List<ThreatAction> Actions = new List<ThreatAction>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
