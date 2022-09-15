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

    public OffHand CloneOneHand(bool inject = false)
    {
        OffHand newOffHand = (OffHand)CloneEquip("OffHand", inject);

        newOffHand.Type = Type;

        return newOffHand;
    }

}
