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

    public override CharacterAbility GenerateAbility(RootOptions options)
    {
        //options.Root = "SummonAbility";
        SummonAbility newAbility = (SummonAbility)GenerateRootObject(options);
        newAbility.Copy(this, options);
        return newAbility;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is SummonAbility))
            return;

        SummonAbility summonSource = (SummonAbility)source;

        SummonPrefab = summonSource.SummonPrefab;
        Quantity = summonSource.Quantity;
        LifeSpan = summonSource.LifeSpan;
    }
}
