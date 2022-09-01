using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/RawItem")]
public class ItemObject : ScriptableObject
{
    [Header("Item Properties")]
    public int itemID;
    public string Name;
    public Sprite Sprite;

    [Header("Fluff")]
    public Quality Quality;
    public int GoldValue;
    public float Weight;

    public virtual ItemObject CloneItem()
    {
        ItemObject newItemObject = (ItemObject)ScriptableObject.CreateInstance("ItemObject");

        newItemObject.itemID = itemID ;
        newItemObject.Name = Name;
        newItemObject.Sprite = Sprite;
        newItemObject.Quality = Quality;
        newItemObject.GoldValue = GoldValue;
        newItemObject.Weight = Weight;

        return newItemObject;
    }
}

public enum Quality
{
    POOR,
    COMMON,
    UNCOMMON,
    RARE,
    LEGENDARY,
    UNIQUE
}
