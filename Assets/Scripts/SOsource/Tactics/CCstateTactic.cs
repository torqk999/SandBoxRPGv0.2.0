using UnityEngine;

[CreateAssetMenu(fileName = "CCstateTactic", menuName = "ScriptableObjects/Tactic/CCstate")]

public class CCstateTactic : Tactic
{
    public CCstateReflection Reflection;
    public bool AND_OR;
    public bool[] CCstates;

    public CCstateTactic(Relation relation, ProcAbility ability,
        bool and_or = true, bool enabled = true) : base(relation, ability, enabled)
    {
        CCstates = new bool[CharacterMath.STATS_CC_COUNT];
        AND_OR = and_or;
    }

    public override void Init()
    {
        Reflection.Reflect(ref CCstates, true);
        //base.Init();
    }
}
