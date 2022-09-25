using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tactic", menuName = "ScriptableObjects/RawTactic")]
public class Tactic : ScriptableObject
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
