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

    public OneHand CloneOneHand(int equipId, bool inject = false)
    {
        OneHand newOneHand = (OneHand)CloneEquip("OneHand", equipId, inject);

        newOneHand.Type = Type;

        return newOneHand;
    }
}