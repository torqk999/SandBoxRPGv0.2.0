using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OffHandType
{
    PARRY,
    TOTEM,
    RELIC,
    TORCH
}

[CreateAssetMenu(fileName = "OffHand", menuName = "ScriptableObjects/Equipment/OffHand")]
public class OffHand : Hand
{
    [Header("OffHand Properties")]
    public OffHandType Type;

    public override Equipment GenerateCloneEquip(int equipId = -1, bool inject = false, string instanceType = "OffHand")
    {
        OffHand newOffHand = (OffHand)base.GenerateCloneEquip(equipId, inject, instanceType);
        newOffHand.Type = Type;
        return newOffHand;
    }

}
