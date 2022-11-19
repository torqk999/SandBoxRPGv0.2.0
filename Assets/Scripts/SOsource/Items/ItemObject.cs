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

    public override DraggableButton GenerateMyButton(ButtonOptions options)
    {
        options.Type = ButtonType.DRAG;
        GameObject buttonObject = GameState.UIman.GenerateButtonObject(options);
        return buttonObject.AddComponent<InventoryButton>();
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "ItemObject" : options.ClassID;
        ItemObject newRoot = (ItemObject)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
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
