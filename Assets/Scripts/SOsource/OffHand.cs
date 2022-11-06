using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OffHandType
{
    PARRY,
    SHIELD,
    BUCKLER
}

[CreateAssetMenu(fileName = "OffHand", menuName = "ScriptableObjects/Equipment/OffHand")]
public class OffHand : Hand
{
    [Header("OffHand Properties")]
    public OffHandType Type;

    public OffHand CloneOffHand(int equipId = -1, bool inject = false)
    {
        OffHand newOffHand = (OffHand)CloneEquip("OffHand", equipId, inject);

        newOffHand.Type = Type;

        return newOffHand;
    }

}
