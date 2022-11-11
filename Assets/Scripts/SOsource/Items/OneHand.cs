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

    public override Equipment GenerateCloneEquip(int equipId = -1, bool inject = false, string instanceType = "OneHand")
    {
        OneHand newOneHand = (OneHand)base.GenerateCloneEquip(equipId, inject, instanceType);

        newOneHand.Type = Type;

        return newOneHand;
    }
}