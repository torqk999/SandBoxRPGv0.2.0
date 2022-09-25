using UnityEngine;

[CreateAssetMenu(fileName = "FloatTactic", menuName = "ScriptableObjects/Tactic/Float")]
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
