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
public class OneHand : Equipment
{
    [Header("OneHand Properties")]
    public OneHandType Type;

    public OneHand CloneOneHand(bool inject = false)
    {
        OneHand newOneHand = (OneHand)CloneEquip("OneHand", inject);

        newOneHand.Type = Type;

        return newOneHand;
    }
}