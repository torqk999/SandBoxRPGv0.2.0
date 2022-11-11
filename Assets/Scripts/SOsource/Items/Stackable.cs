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

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        Stackable newItemObject = (Stackable)CreateInstance("Stackable");
        newItemObject.CloneItem(this);
        return newItemObject;
    }

    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is Stackable))
            return;

        Stackable stackSource = (Stackable)source;

        MaxQuantity = stackSource.MaxQuantity;
        quantity = Math.Abs(quantity);
        quantity = quantity > MaxQuantity ? MaxQuantity : quantity;
        CurrentQuantity = quantity;
    }
}
