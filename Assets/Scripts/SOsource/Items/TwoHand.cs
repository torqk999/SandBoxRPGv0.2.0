using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwoHandType
{
    AXE,
    CLAYMORE,
    POLEARM,
    BARDICHE,
    SPEAR,
    STAFF,
    BOW,
    CROSSBOW
}

[CreateAssetMenu(fileName = "TwoHand", menuName = "ScriptableObjects/Equipment/TwoHand")]
public class TwoHand : Hand
{
    [Header("TwoHand Properties")]
    public TwoHandType Type;

    public override Equipment GenerateCloneEquip(int equipId, bool inject = false, string instanceType = "TwoHand")
    {
        TwoHand newTwoHand = (TwoHand)base.GenerateCloneEquip(equipId, inject, instanceType);
        newTwoHand.Type = Type;
        return newTwoHand;
    }
}