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

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = "SummonAbility";
        SummonAbility newAbility = (SummonAbility)GenerateRootObject(options);
        newAbility.Clone(options);
        return newAbility;
    }
    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is SummonAbility))
            return;

        SummonAbility summonSource = (SummonAbility)options.Source;

        SummonPrefab = summonSource.SummonPrefab;
        Quantity = summonSource.Quantity;
        LifeSpan = summonSource.LifeSpan;
    }
}
