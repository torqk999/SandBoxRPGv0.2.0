using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonAbility", menuName = "ScriptableObjects/Abilities/Summon")]
public class SummonAbility : CharacterAbility
{
    [Header("Summon Properties")]
    public Character SummonPrefab;
    public int Quantity;
    public float LifeSpan;

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source, equipId);

        if (!(source is SummonAbility))
            return;

        SummonAbility summonSource = (SummonAbility)source;

        
        SummonPrefab = summonSource.SummonPrefab;
        Quantity = summonSource.Quantity;
        LifeSpan = summonSource.LifeSpan;
    }

    public override CharacterAbility GenerateAbility(float potency = 1, bool inject = false, int equipId = 0)
    {
        SummonAbility newAbility = (SummonAbility)CreateInstance("SummonAbility");
        newAbility.CloneAbility(this, equipId, potency, inject);
        return newAbility;
    }
}
