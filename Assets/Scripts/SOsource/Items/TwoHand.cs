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

    public TwoHand CloneTwoHand(int equipId, bool inject = false)
    {
        TwoHand newTwoHand = (TwoHand)CloneEquip("TwoHand", equipId, inject);

        newTwoHand.Type = Type;

        return newTwoHand;
    }
}