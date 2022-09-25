using UnityEngine;

[CreateAssetMenu(fileName = "ProxyTactic", menuName = "ScriptableObjects/Tactic/Proxy")]
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
