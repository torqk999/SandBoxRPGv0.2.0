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
    public string Flavour;
    public Sprite Sprite;

    [Header("Fluff")]
    public Quality Quality;
    public int GoldValue;
    public float Weight;

    public virtual ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        ItemObject newItemObject = (ItemObject)CreateInstance("ItemObject");
        newItemObject.CloneItem(this);
        newItemObject.InitializeSource();
        return newItemObject;
    }
    public virtual void InitializeSource()
    {

    }

    public virtual void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        itemID = source.itemID;
        Name = source.Name;
        Sprite = source.Sprite;
        Quality = source.Quality;
        GoldValue = source.GoldValue;
        Weight = source.Weight;
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
