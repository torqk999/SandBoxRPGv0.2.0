using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stackable", menuName = "ScriptableObjects/Stackable")]
public class Stackable : ItemObject
{
    [Header("Stackable")]
    public int MaxQuantity;
    public int CurrentQuantity;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = options.Root == "" ? "Stackable" : options.Root;
        Stackable newRoot = (Stackable)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }

    public override void Clone(RootOptions options)
    {
        if (!(options.Source is Stackable))
            return;

        Stackable stackSource = (Stackable)options.Source;

        MaxQuantity = stackSource.MaxQuantity;
        CurrentQuantity = Math.Abs(options.Quantity);
        CurrentQuantity = CurrentQuantity > MaxQuantity ? MaxQuantity : CurrentQuantity;
    }
}
