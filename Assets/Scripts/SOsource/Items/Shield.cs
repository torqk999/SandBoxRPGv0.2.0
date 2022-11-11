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

    public override Equipment GenerateCloneEquip(int equipId, bool inject = false, string instanceType = "Shield")
    {
        Shield newShield = (Shield)base.GenerateCloneEquip(equipId, inject, instanceType);
        newShield.Type = Type;
        return newShield;
    }
}
