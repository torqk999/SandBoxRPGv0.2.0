using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldType
{
    BUCKLER,
    WOODEN,
    TOWER,
    KITE
}

[CreateAssetMenu(fileName = "Shield", menuName = "ScriptableObjects/Equipment/Shield")]
public class Shield : Hand
{
    [Header("Equipment Properties")]
    public ShieldType Type;

    public Shield CloneShield(int equipId, bool inject = false)
    {
        Shield newShield = (Shield)CloneEquip("Shield", equipId, inject);

        newShield.Type = Type;

        return newShield;
    }
}