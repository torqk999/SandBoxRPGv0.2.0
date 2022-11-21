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
    public Page Inventory;
    public Page Equips;
    public Page HotBar;
    public Page Skills;
}
