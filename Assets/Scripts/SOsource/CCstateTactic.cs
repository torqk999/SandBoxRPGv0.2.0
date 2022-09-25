using UnityEngine;

[CreateAssetMenu(fileName = "CCstateTactic", menuName = "ScriptableObjects/Tactic/CCstate")]
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
