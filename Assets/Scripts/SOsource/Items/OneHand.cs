using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OneHandType
{
    DAGGER,
    WAND,
    RAPIER,
    AXE,
    MACE,
    FLAIL
}

[CreateAssetMenu(fileName = "OneHand", menuName = "ScriptableObjects/Equipment/OneHand")]
public class OneHand : Hand
{
    [Header("OneHand Properties")]
    public OneHandType Type;

    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        EquipSlot = EquipSlot.MAIN;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = options.Root == "" ? "OneHand" : options.Root;
        OneHand newRoot = (OneHand)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is OneHand))
            return;

        OneHand oneSource = (OneHand)source;
        Type = oneSource.Type;

        Debug.Log("Copy OneHand Complete!");
    }
}