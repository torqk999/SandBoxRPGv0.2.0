using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/RawItem")]
public class ItemObject : RootScriptObject
{
    [Header("Item Properties")]
    public Quality Quality;
    public int GoldValue;
    public float Weight;

    [Header("Item Logic")]
    public int SlotIndex;
    public Equipment[] SlotFamily;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "ItemObject" : options.ClassID;
        ItemObject newRoot = (ItemObject)base.GenerateRootObject(options);
        newRoot.Clone(this, options);
        return newRoot;
    }
    public virtual void InitializeRoot()
    {

    }

    public override void Clone(RootScriptObject source, RootOptions options)
    {
        base.Clone(source, options);

        if (!(source is ItemObject))
            return;

        ItemObject item = (ItemObject)source;

        Quality = item.Quality;
        GoldValue = item.GoldValue;
        Weight = item.Weight;
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
