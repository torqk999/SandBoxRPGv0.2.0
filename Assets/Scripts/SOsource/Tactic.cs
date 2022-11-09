using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tactic", menuName = "ScriptableObjects/RawTactic")]
public class Tactic : ScriptableObject
{
    public bool bTacticEnabled;
    public Relation Relation;
    public GenericAbility Ability;

    public Tactic(Relation relation, GenericAbility ability, bool enabled = true)
    {
        Relation = relation;
        Ability = ability;
        bTacticEnabled = enabled;
    }

    public virtual void Init()
    {

    }
}
