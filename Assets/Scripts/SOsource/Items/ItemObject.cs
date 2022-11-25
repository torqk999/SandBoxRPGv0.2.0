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

    //[Header("Item Logic")]
    //public int SlotIndex;
    //public SlotPage SlotPage;

    public override RootButton GenerateMyButton(ButtonOptions options)
    {
        options.ButtonType = ButtonType.ITEM;
        GameObject buttonObject = RootLogic.GameState.UIman.GenerateButtonObject(options);
        RootButton myButton = buttonObject.AddComponent<RootButton>();
        myButton.Init(options);
        return myButton;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = options.Root == "" ? "ItemObject" : options.Root;
        ItemObject newRoot = (ItemObject)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        newRoot.Occupy(options.Page, options.Index);
        return newRoot;
    }
    public virtual void InitializeRoot()
    {

    }

    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

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
