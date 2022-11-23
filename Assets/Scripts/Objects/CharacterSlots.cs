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
    public Panel Inventory;
    public Panel Equips;
    public Panel HotBar;
    public Panel Skills;
}
