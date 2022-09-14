using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stackable", menuName = "ScriptableObjects/Stackable")]
public class Stackable : ItemObject
{
    [Header("Stackable")]
    public int MaxQuantity;

    public virtual Stackable CloneStackable()
    {
        Stackable newStackable = (Stackable)CloneItem("Stackable");

        newStackable.MaxQuantity = MaxQuantity;

        return newStackable;
    }
}
