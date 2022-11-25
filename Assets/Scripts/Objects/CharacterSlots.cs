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
    public RootPanel Inventory;
    public RootPanel Equips;
    public RootPanel HotBar;
    public RootPanel Skills;
}
