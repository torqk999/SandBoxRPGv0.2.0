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
        newRoot.Copy(this, options);
        return newRoot;
    }

    public override void Copy(RootScriptObject source, RootOptions options)
    {
        if (!(source is Stackable))
            return;

        Stackable stackSource = (Stackable)source;

        MaxQuantity = stackSource.MaxQuantity;
        CurrentQuantity = Math.Abs(options.Quantity);
        CurrentQuantity = CurrentQuantity > MaxQuantity ? MaxQuantity : CurrentQuantity;
    }
}
