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

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        ItemObject newRoot = (ItemObject)base.GenerateRootObject(options);
        newRoot.Clone(options);
        //newRoot.Occupy(options.HomePanel, options.Index);
        return newRoot;
    }
    public virtual void InitializeRoot()
    {

    }

    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is ItemObject))
            return;

        ItemObject item = (ItemObject)options.Source;

        Quality = item.Quality;
        GoldValue = item.GoldValue;
        Weight = item.Weight;

        Debug.Log("Copy Item Complete!");
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
