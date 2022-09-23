using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Relation
{
    SELF,
    ALLY,
    FOE
}
public enum TacFloatType
{
    STAT_CURRENT,
    STAT_RELATIVE,
    RESISTANCE,
}
public enum TacFixedType
{
    TARGET,
    NEAREST,
    FURTHEST
}
[System.Serializable]
public class Tactic
{
    public bool bTacticEnabled;
    public Relation Relation;
    public CharacterAbility Ability;

    public Tactic(Relation relation, CharacterAbility ability, bool enabled = true)
    {
        Relation = relation;
        Ability = ability;
        bTacticEnabled = enabled;
    }
}
[System.Serializable]
public class FloatTactic : Tactic
{
    public TacFloatType Type;
    public RawStat TargetStat;
    public Element TargetElement;
    public bool GTE;
    public float Value;

    public FloatTactic(Relation relation, CharacterAbility ability, TacFloatType type, float value, RawStat stat = RawStat.HEALTH, Element element = Element.PHYSICAL, bool gte = true,bool enabled = true) : base(relation, ability, enabled)
    {
        Type = type;
        TargetStat = stat;
        TargetElement = element;
        GTE = gte;
        Value = value;
    }
}
[System.Serializable]
public class ProxyTactic : Tactic
{
    public bool Allies;
    public bool Targetted;
    public int Count;
    public float Range;

    public ProxyTactic(Relation relation, CharacterAbility ability, int count, float range, bool allies = true, bool targetted = false, bool enabled = true) : base(relation, ability, enabled)
    {
        Allies = allies;
        Targetted = targetted;
        Count = count;
        Range = range;
    }
}
[System.Serializable]
public class CCstateTactic : Tactic
{
    public bool[] CCstates;
    public bool AND_OR;

    public CCstateTactic(Relation relation, CharacterAbility ability, bool and_or = true, bool enabled = true) : base(relation, ability, enabled)
    {
        CCstates = new bool[CharacterMath.STATS_CC_COUNT];
        AND_OR = and_or;
    }
}
public class FixedTactic : Tactic
{
    public TacFixedType Type;
    public Character Target;

    public FixedTactic(Relation relation, CharacterAbility ability, TacFixedType type, Character target = null, bool enabled = true) : base(relation, ability, enabled)
    {
        Type = type;
        Target = target;
    }
}
[System.Serializable]
public class Strategy
{
    public GameState GameState;
    public Character Character;

    public Tactic[] TacticSlots;
    //public int CurrentlyActiveTacticIndex;

    public Strategy(Character parent, Tactic[] preset = null)
    {
        Character = parent;
        GameState = parent.GameState;
        TacticSlots = new Tactic[CharacterMath.TACTIC_SLOTS];
        if (preset != null)
            for (int i = 0; i < preset.Length && i < CharacterMath.TACTIC_SLOTS; i++)
                TacticSlots[i] = preset[i];
        //CurrentlyActiveTacticIndex = 0;
    }
    public void UpdateCurrentTactic()
    {
        for (int i = 0; i < TacticSlots.Length; i++)
        {
            if (!TacticSlots[i].bTacticEnabled)
                continue;

            if (!CheckTacticConditions(TacticSlots[i]))
                continue;
        }
    }

    bool CheckTacticConditions(Tactic tactic)
    {
        if (!GameState.CharacterMan.CheckAbility(tactic.Ability, Character, Character.GenerateStatModifier(tactic.Ability.CostType, tactic.Ability.CostTarget)))
            return false;

        List<Character> candidates = GetDesiredCharacters(tactic);

        //if (tactic.Condition == TacCondition.GRT)
            Character.CurrentTargetCharacter = ReturnMostCharacter(tactic, candidates);

        //else if (tactic.Condition == TacCondition.LST)
            Character.CurrentTargetCharacter = ReturnLeastCharacter(tactic, candidates);

        //else
            Character.CurrentTargetCharacter = ReturnIdealCharacter(tactic, candidates);

        return (Character.CurrentTargetCharacter != null);
    }
    List<Character> GetDesiredCharacters(Tactic tactic)
    {
        List<Character> desiredCharacters = new List<Character>();
        //targetCharacter = null;

        if (tactic.Relation == Relation.SELF)
            desiredCharacters.Add(Character);

        //else if (tactic.Target == ThreatRelation.TARGET)
        //    desiredCharacters.Add(tactic.SpecificTarget);

        else
            foreach (Party party in GameState.CharacterMan.Parties)
            {
                if (tactic.Relation == Relation.ALLY &&
                    party.Faction != Character.Sheet.Faction)
                    continue;

                if (tactic.Relation == Relation.FOE &&
                    party.Faction == Character.Sheet.Faction)
                    continue;

                desiredCharacters.AddRange(party.Members);
            }

        return desiredCharacters;
    }

    void GetApplicableCharacters(Tactic tactic, List<Character> characterPool)
    {
        //if (tactic.Condition == TacCondition.GRT)

        //if (tactic.Condition == TacCondition.LST)

        for (int i = characterPool.Count - 1; i > -1; i--)
        {
            //float check;

        }
    }
    Character ReturnMostCharacter(Tactic tactic, List<Character> characterPool)
    {
        Character output = null;


        return output;
    }
    Character ReturnLeastCharacter(Tactic tactic, List<Character> characterPool)
    {
        Character output = null;


        return output;
    }
    Character ReturnIdealCharacter(Tactic tactic, List<Character> characterPool)
    {
        Character output = null;

        GetApplicableCharacters(tactic, characterPool);
        GetCharactersInRange(tactic, characterPool);

        if (characterPool.Count > 0)
            output = characterPool[0];

        return output;
    }

    /*
    bool PerformFloatConditional(Tactic tactic, Character character)
    {
        float value;
        float[] data;

        switch (tactic.Type)
        {
            case TacType.RESISTANCE:
                data = character.Resistances.PullData();
                value = data[(int)tactic.Resistance];
                break;

            case TacType.STAT_CURRENT:
                data = character.CurrentStats.PullData();
                value = data[(int)tactic.CharStat];
                break;

            case TacType.STAT_MAX:
                data = character.MaximumStatValues.PullData();
                value = data[(int)tactic.CharStat];
                break;
        }

        switch (tactic.Condition)
        {
            case TacCondition.GRT:
                break;

            case TacCondition.LST:
                break;
        }
    }
    bool PerformStateConditional(Tactic tactic, Character character)
    {
        bool state;
        switch (tactic.Type)
        {
            case TacType.ALIVE:
                return (tactic.State != character.bIsAlive);

            case TacType.CC_STATE:

        }
    }
    */
    void GetCharactersInRange(Tactic tactic, List<Character> characterPool)
    {

    }
}