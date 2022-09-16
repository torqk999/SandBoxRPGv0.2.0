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
public class OffHand : Equipment
{
    [Header("OffHand Properties")]
    public OffHandType Type;

    public OffHand CloneOneHand(int equipId = -1, bool inject = false)
    {
        OffHand newOffHand = (OffHand)CloneEquip("OffHand", equipId, inject);

        newOffHand.Type = Type;

        return newOffHand;
    }

}
