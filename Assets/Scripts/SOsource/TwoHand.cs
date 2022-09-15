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
public class TwoHand : Equipment
{
    [Header("TwoHand Properties")]
    public TwoHandType Type;

    public TwoHand CloneTwoHand(bool inject = false)
    {
        TwoHand newTwoHand = (TwoHand)CloneEquip("TwoHand",inject);

        newTwoHand.Type = Type;

        return newTwoHand;
    }
}