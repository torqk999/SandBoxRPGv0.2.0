using System;
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
//[System.Serializable]
public class Strategy : MonoBehaviour
{
    public GameState GameState;
    public Character Character;
    List<Character> CandidateBuffer = new List<Character>();

    public Tactic[] TacticSlots;
    bool[] CCstateBuffer = new bool[CharacterMath.STATS_CC_COUNT];
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

            Character.CurrentAction = ReturnComparableAbility(TacticSlots[i].Ability);

            if (Character.CurrentAction == null)
                continue;

            if (!CheckTacticConditions(TacticSlots[i])) // <<< Target Assignment
                continue;

            return;
        }
        Character.CurrentAction = null;
    }
    CharacterAbility ReturnComparableAbility(CharacterAbility source)
    {
        foreach (AbilityButton abilityButton in Character.Slots.Skills.List) // Check all abilities
        {
            CharacterAbility ability = (CharacterAbility)abilityButton.Root;

            if (ability.School != source.School)
                continue;

            switch(source)
            {
                case EffectAbility:
                    if (!(ability is EffectAbility))
                        continue;

                    EffectAbility effectSource = (EffectAbility)source;
                    EffectAbility effectAbility = (EffectAbility)ability;

                    foreach (BaseEffect effect in effectSource.Effects) // Check for atleast one matching effect
                    {
                        if (CheckForComparableEffect(effectAbility, effect))
                            return ability;
                    }
                    break;
            }
        }
        return null;
    }
    bool CheckForComparableEffect(EffectAbility target, BaseEffect source)
    {
        //BaseEffect effect = Array.Find(target.Effects, x => x.Action == source.Action);
        //if (effect == null)
        //    return false;

        /*switch(effect.Action)
        {
            case EffectAction.DMG_HEAL:
                if (effect.Duration != source.Duration
                 || effect.TargetStat != source.TargetStat
                 || effect.Value != source.Value)
                    return false;
                return true;

            case EffectAction.SPAWN:
                return true;

            case EffectAction.CC_CLEANSE:
                if (effect.TargetCCstatus != source.TargetCCstatus)
                    return false;
                return true;

            case EffectAction.CROWD_CONTROL:
                if (effect.TargetCCstatus != source.TargetCCstatus)
                    return false;
                return true;

            case EffectAction.RES_ADJ:
                if (effect.Duration != source.Duration
                 || effect.TargetElement != source.TargetElement
                 || effect.Value != source.Value)
                    return false;
                return true;

            case EffectAction.STAT_ADJ:
                if (effect.Duration != source.Duration
                 || effect.TargetStat != source.TargetStat
                 || effect.Value != source.Value)
                    return false;
                return true;
        }*/
        return false;
    }
    bool CheckTacticConditions(Tactic tactic)
    {
        float costModifier = Character.GenerateRawStatValueModifier(tactic.Ability.CostType, tactic.Ability.CostTarget);

        if (!Character.CheckCanCastAbility(tactic.Ability, costModifier))
            return false;

        ReturnEligableCharacters(tactic, ref CandidateBuffer);
        ReturnDesiredCharacters(tactic, ref CandidateBuffer);

        Character.CurrentTargetCharacter = ReturnIdealCharacter(ref CandidateBuffer); // <<< Target Assignment

        return (Character.CurrentTargetCharacter != null);
    }
    void ReturnEligableCharacters(Tactic tactic, ref List<Character> pool)
    {
        pool.Clear();

        switch(tactic.Relation)
        {
            case Relation.SELF:
                pool.Add(Character);
                break;

            case Relation.ALLY:
            case Relation.FOE:
                Party party = GameState.CharacterMan.Parties.Find(x => x.Faction == Character.Sheet.Faction);
                if (party == null)
                {
                    Debug.Log("Invalid party!");
                    return;
                }
                pool.AddRange(tactic.Relation == Relation.ALLY ? Character.CurrentParty.Members : Character.CurrentParty.Foes);
                break;
        }
    }
    void ReturnDesiredCharacters(Tactic tactic, ref List<Character> eligable)
    {
        switch(tactic)
        {
            case FixedTactic:
                ReturnFixedCharacter((FixedTactic)tactic, ref eligable);
                break;

            case FloatTactic:
                ReturnFloatCharacters((FloatTactic)tactic, ref eligable);
                break;

            case CCstateTactic:
                ReturnCCstateCharacters((CCstateTactic)tactic, ref eligable);
                break;

            case ProxyTactic:
                ReturnProxyCharacters((ProxyTactic)tactic, ref eligable);
                break;
        }
    }
    void ReturnFixedCharacter(FixedTactic tactic, ref List<Character> eligable)
    {
        Character target = null;
        switch (tactic.Type)
        {
            case TacFixedType.TARGET:
                target = eligable.Find(x => x == tactic.Target);               
                break;

            case TacFixedType.NEAREST:
            case TacFixedType.FURTHEST:
                float best = 0;
                foreach(Character next in eligable)
                {
                    float distance = Vector3.Distance(next.Root.position, Character.Root.position);
                    if (target == null
                        || (tactic.Type == TacFixedType.NEAREST && distance < best)
                        || (tactic.Type == TacFixedType.FURTHEST && distance > best))
                    {
                        target = next;
                        best = distance;
                    }
                }
                break;
        }
        eligable.Clear();
        eligable.Add(target);
    }
    void ReturnFloatCharacters(FloatTactic tactic, ref List<Character> eligable)
    {
        foreach (Character next in GameState.CharacterMan.CharacterPool)
        {
            switch (tactic.Type)
            {
                case TacFloatType.STAT_CURRENT:

                    if ((tactic.Relative &&
                       (tactic.GTE_LT == (next.CurrentStats.Stats[(int)tactic.TargetStat] >= Character.CurrentStats.Stats[(int)tactic.TargetStat])))
                    ||
                        (!tactic.Relative &&
                       (tactic.GTE_LT == (next.CurrentStats.Stats[(int)tactic.TargetStat] >= tactic.Value))))
                    {
                        break;
                    }
                    eligable.Remove(next);
                    break;

                case TacFloatType.STAT_MAXIMUM:
                    if ((tactic.Relative &&
                       (tactic.GTE_LT == (next.MaximumStatValues.Stats[(int)tactic.TargetStat] >= Character.MaximumStatValues.Stats[(int)tactic.TargetStat])))
                    ||
                        (!tactic.Relative &&
                       (tactic.GTE_LT == (next.MaximumStatValues.Stats[(int)tactic.TargetStat] >= tactic.Value))))
                    {
                        break;
                    }
                    eligable.Remove(next);
                    break;

                case TacFloatType.RESISTANCE:
                    if ((tactic.Relative &&
                       (tactic.GTE_LT == (next.CurrentResistances.Elements[(int)tactic.TargetElement] >= Character.CurrentResistances.Elements[(int)tactic.TargetElement])))
                    ||
                        (!tactic.Relative &&
                       (tactic.GTE_LT == (next.CurrentResistances.Elements[(int)tactic.TargetElement] >= tactic.Value))))
                    {
                        break;
                    }
                    eligable.Remove(next);
                    break;
            }
        }
    }
    void ReturnProxyCharacters(ProxyTactic tactic, ref List<Character> eligable)
    {
        foreach (Character source in eligable)
        {
            source.CountBuffer = 0;
            foreach (Character target in eligable)
            {
                if (tactic.Allies != (target.Sheet.Faction == source.Sheet.Faction))
                    continue;

                if (tactic.Targetted && target.CurrentTargetCharacter == source)
                    source.CountBuffer++;

                if (!tactic.Targetted && Vector3.Distance(source.Root.position, target.Root.position) <= tactic.Range)
                    source.CountBuffer++;
            }
        }

        for (int i = eligable.Count - 1; i > -1; i--)
        {
            if (eligable[i].CountBuffer < tactic.Count)
                eligable.RemoveAt(i);
        }
    }
    void ReturnCCstateCharacters(CCstateTactic tactic, ref List<Character> eligable)
    {
        foreach (Character source in eligable)
        {
            Array.Fill(CCstateBuffer, false);
            foreach (CrowdControlEffect effect in source.Risiduals)
                //if (effect.Action == EffectAction.CROWD_CONTROL)
                CCstateBuffer[(int)effect.TargetCCstatus] = true;

            bool check = tactic.AND_OR;
            for (int i = 0; i < CharacterMath.STATS_CC_COUNT; i++)
            {
                if (tactic.AND_OR && tactic.CCstates[i] && !CCstateBuffer[i])
                    check = false;

                if (!tactic.AND_OR && tactic.CCstates[i] && CCstateBuffer[i])
                    check = true;
            }

            if (!check)
                eligable.Remove(source);
        }
    }
    Character ReturnIdealCharacter(/*Tactic tactic, */ref List<Character> characterPool)
    {
        Character output = null;

        // Lazy approach :S
        if (characterPool.Count > 0)
            output = characterPool[0];

        return output;
    }

    private void Start()
    {
        foreach(Tactic tac in TacticSlots)
        {
            if (tac != null)
            {
                tac.Init();
            }
        }
    }
    private void Update()
    {
        if (GameState == null)
            return;
        UpdateCurrentTactic();
    }
}