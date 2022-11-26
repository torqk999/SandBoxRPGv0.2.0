using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tactic", menuName = "ScriptableObjects/RawTactic")]
public class Tactic : RootScriptObject
{
    public bool bTacticEnabled;
    public Relation Relation;
    public ProcAbility Ability;

    public Tactic(Relation relation, ProcAbility ability, bool enabled = true)
    {
        Relation = relation;
        Ability = ability;
        bTacticEnabled = enabled;
    }

    public virtual void Init()
    {

    }
}
