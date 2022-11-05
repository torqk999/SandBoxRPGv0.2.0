using UnityEngine;

[CreateAssetMenu(fileName = "FixedTactic", menuName = "ScriptableObjects/Tactic/Fixed")]
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
