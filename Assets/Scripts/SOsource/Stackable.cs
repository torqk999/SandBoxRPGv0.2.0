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
        Stackable newStackable = (Stackable)ScriptableObject.CreateInstance("Stackable");

        newStackable.itemID = itemID;
        newStackable.Name = Name;
        newStackable.Sprite = Sprite;
        newStackable.Quality = Quality;
        newStackable.GoldValue = GoldValue;
        newStackable.Weight = Weight;

        newStackable.MaxQuantity = MaxQuantity;

        return newStackable;
    }
}
