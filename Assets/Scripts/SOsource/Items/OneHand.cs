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
        OneHand newRoot = (OneHand)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }
    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is OneHand))
            return;

        OneHand oneSource = (OneHand)options.Source;
        Type = oneSource.Type;

        Debug.Log("Copy OneHand Complete!");
    }
}