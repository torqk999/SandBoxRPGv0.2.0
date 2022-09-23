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
    STAT_MAXIMUM,
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
    public bool GTE_LT;
    public bool Relative;
    public float Value;

    public FloatTactic(Relation relation, CharacterAbility ability,
        TacFloatType type, float value, RawStat stat = RawStat.HEALTH, Element element = Element.PHYSICAL,
        bool gte = true, bool relative = false, bool enabled = true) : base(relation, ability, enabled)
    {
        Type = type;
        TargetStat = stat;
        TargetElement = element;
        GTE_LT = gte;
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

    public ProxyTactic(Relation relation, CharacterAbility ability, int count, float range,
        bool allies = true, bool targetted = false, bool enabled = true) : base(relation, ability, enabled)
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

    public CCstateTactic(Relation relation, CharacterAbility ability,
        bool and_or = true, bool enabled = true) : base(relation, ability, enabled)
    {
        CCstates = new bool[CharacterMath.STATS_CC_COUNT];
        AND_OR = and_or;
    }
}
[System.Serializable]
public class FixedTactic : Tactic
{
    public TacFixedType Type;
    public Character Target;

    public FixedTactic(Relation relation, CharacterAbility ability, TacFixedType type, Character target = null,
        bool enabled = true) : base(relation, ability, enabled)
    {
        Type = type;
        Target = target;
    }
}
//[System.Serializable]
public class Strategy : MonoBehaviour
{
    public GameState GameState;
    public Character Character;

    public Tactic[] TacticSlots;
    //public CharacterAbility CurrentAction;

    public Strategy(Character parent, Tactic[] preset = null)
    {
        Character = parent;
        GameState = parent.GameState;
        TacticSlots = new Tactic[CharacterMath.TACTIC_SLOTS];
        if (preset != null)
            for (int i = 0; i < preset.Length && i < CharacterMath.TACTIC_SLOTS; i++)
                TacticSlots[i] = preset[i];
    }
    void UpdateCurrentTactic()
    {
        for (int i = 0; i < TacticSlots.Length; i++)
        {
            if (!TacticSlots[i].bTacticEnabled)
                continue;

            if (!CheckTacticConditions(TacticSlots[i]))
                continue;

            Character.CurrentAction = TacticSlots[i].Ability;
            break;
        }
        Character.CurrentAction = null;
    }
    bool CheckTacticConditions(Tactic tactic)
    {
        if (!GameState.CharacterMan.CheckAbility(tactic.Ability, Character, Character.GenerateStatModifier(tactic.Ability.CostType, tactic.Ability.CostTarget)))
            return false;

        List<Character> candidates = ReturnDesiredCharacters(tactic);

        Character.CurrentTargetCharacter = ReturnIdealCharacter(tactic, candidates);

        return (Character.CurrentTargetCharacter != null);
    }
    List<Character> ReturnDesiredCharacters(Tactic tactic)
    {
        List<Character> desiredCharacters = new List<Character>();
        //targetCharacter = null;

        switch(tactic)
        {
            case FixedTactic:
                desiredCharacters.Add(ReturnFixedCharacter((FixedTactic)tactic));
                break;

            case FloatTactic:
                desiredCharacters.AddRange(ReturnFloatCharacters((FloatTactic)tactic));
                break;

            case CCstateTactic:
                desiredCharacters.AddRange(ReturnCCstateCharacters((CCstateTactic)tactic));
                break;

            case ProxyTactic:
                desiredCharacters.AddRange(ReturnProxyCharacters((ProxyTactic)tactic));
                break;
        }
        
        return desiredCharacters;
    }
    Character ReturnFixedCharacter(FixedTactic tactic)
    {
        switch(tactic.Type)
        {
            case TacFixedType.TARGET:
                return tactic.Target;

            case TacFixedType.NEAREST:
            case TacFixedType.FURTHEST:
                Character fix = null;
                float best = 0;
                foreach(Character next in GameState.CharacterMan.CharacterPool)
                {
                    float distance = Vector3.Distance(next.Root.position, Character.Root.position);
                    if (fix == null
                        || (tactic.Type == TacFixedType.NEAREST && distance < best)
                        || (tactic.Type == TacFixedType.FURTHEST && distance > best))
                    {
                        fix = next;
                        best = distance;
                    }
                }
                return fix;
        }
        Debug.Log("ReturnFixedCharacter Failed!");
        return null;
    }
    List<Character> ReturnFloatCharacters(FloatTactic tactic)
    {
        List<Character> floatCandidates = new List<Character>();
        foreach (Character next in GameState.CharacterMan.CharacterPool)
        {
            switch (tactic.Type)
            {
                case TacFloatType.STAT_CURRENT:
                    if ((tactic.Relative &&
                       ((tactic.GTE_LT && (next.CurrentStats.Stats[(int)tactic.TargetStat] >= Character.CurrentStats.Stats[(int)tactic.TargetStat]))
                    || (!tactic.GTE_LT && (next.CurrentStats.Stats[(int)tactic.TargetStat] < Character.CurrentStats.Stats[(int)tactic.TargetStat]))))
                    || (!tactic.Relative &&
                       ((tactic.GTE_LT && (next.CurrentStats.Stats[(int)tactic.TargetStat] >= tactic.Value))
                    || (!tactic.GTE_LT && (next.CurrentStats.Stats[(int)tactic.TargetStat] < tactic.Value)))))
                    {
                        goto Append;
                        //floatCandidates.Add(next);
                    }
                    break;

                case TacFloatType.STAT_MAXIMUM:
                    break;

                case TacFloatType.RESISTANCE:
                    break;
            }
            continue;
        Append:;
            floatCandidates.Add(next);

        }
        
        return floatCandidates;
    }
    List<Character> ReturnProxyCharacters(ProxyTactic tactic)
    {
        List<Character> floatCandidates = new List<Character>();

        return floatCandidates;
    }
    List<Character> ReturnCCstateCharacters(CCstateTactic tactic)
    {
        List<Character> floatCandidates = new List<Character>();

        return floatCandidates;
    }
    Character ReturnIdealCharacter(Tactic tactic, List<Character> characterPool)
    {
        Character output = null;

        //GetApplicableCharacters(tactic, characterPool);
        GetCharactersInRange(tactic, characterPool);

        if (characterPool.Count > 0)
            output = characterPool[0];

        return output;
    }
    void GetCharactersInRange(Tactic tactic, List<Character> characterPool)
    {

    }

    private void Start()
    {
        
    }
    private void Update()
    {
        UpdateCurrentTactic();
    }
}