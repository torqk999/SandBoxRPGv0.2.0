using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*public enum TacTarget
{
    SELF,
    TARGET,
    ALLY,
    FOE
}*/
public enum TacCondition
{
    GT,
    GTE,
    GRT,
    LT,
    LTE,
    LST,
    IS,
    NOT
}
public enum TacType
{
    //RANGE_MIN,
    //RANGE_MAX,
    ALIVE,
    CC_STATE,
    STAT_CURRENT,
    STAT_MAX,
    RESISTANCE,
    TARGETTING // TARGETTED_BY??
}
public enum TacStat
{
    HEALTH,
    STAMINA,
    MANA
}
[System.Serializable]
public struct Tactic
{
    public bool bTacticEnabled;

    public ThreatRelation Target;
    public Element Resistance;
    public TacStat CharStat;
    public TacType Type;
    public TacCondition Condition;
    public float Trigger;
    public bool State;

    public CharacterAbility Ability;
    public Character SpecificTarget;
}
[System.Serializable]
public class Strategy
{
    public GameState GameState;
    public Character Character;

    public Tactic[] TacticSlots;
    public int CurrentlyActiveTacticIndex;
    public bool bTargetAssigned;

    public Strategy(Character parent, Tactic[] preset = null)
    {
        Character = parent;
        GameState = parent.GameState;
        TacticSlots = new Tactic[CharacterMath.TACTIC_SLOTS];
        if (preset != null)
            for (int i = 0; i < preset.Length && i < CharacterMath.TACTIC_SLOTS; i++)
                TacticSlots[i] = preset[i];
        CurrentlyActiveTacticIndex = 0;
        bTargetAssigned = false;
    }
    public void UpdateCurrentTactic()
    {
        bTargetAssigned = false;

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

        if (tactic.Condition == TacCondition.GRT)
            Character.CurrentTargetCharacter = ReturnMostCharacter(tactic, candidates);

        else if (tactic.Condition == TacCondition.LST)
            Character.CurrentTargetCharacter = ReturnLeastCharacter(tactic, candidates);

        else
            Character.CurrentTargetCharacter = ReturnIdealCharacter(tactic, candidates);

        return (Character.CurrentTargetCharacter != null);
    }
    List<Character> GetDesiredCharacters(Tactic tactic)
    {
        List<Character> desiredCharacters = new List<Character>();
        //targetCharacter = null;

        if (tactic.Target == ThreatRelation.SELF)
            desiredCharacters.Add(Character);

        else if (tactic.Target == ThreatRelation.TARGET)
            desiredCharacters.Add(tactic.SpecificTarget);

        else
            foreach (Party party in GameState.CharacterMan.Parties)
            {
                if (tactic.Target == ThreatRelation.ALLY &&
                    party.Faction != Character.Sheet.Faction)
                    continue;

                if (tactic.Target == ThreatRelation.FOE &&
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