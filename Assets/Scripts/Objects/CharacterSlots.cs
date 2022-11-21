using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SlotPageType
{
    INVENTORY,
    EQUIPMENT,
    HOT_BAR,
    SKILLS
}

[System.Serializable]
public struct CharacterSlots
{
    public ListPanel Inventory;
    public ListPanel Equips;
    public ListPanel HotBar;
    public ListPanel Skills;
}
