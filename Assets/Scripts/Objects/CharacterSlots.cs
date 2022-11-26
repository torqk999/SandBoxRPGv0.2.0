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
    public List<RootScriptObject> Inventory;
    public List<RootScriptObject> Equips;
    public List<RootScriptObject> HotBar;
    public List<RootScriptObject> Skills;
    public List<RootScriptObject> Risiduals;
}
