using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwoHandType
{
    AXE,
    CLAYMORE,
    POLEARM,
    BARDICHE,
    SPEAR,
    STAFF,
    BOW,
    CROSSBOW
}

[CreateAssetMenu(fileName = "TwoHand", menuName = "ScriptableObjects/Equipment/TwoHand")]
public class TwoHand : Hand
{
    [Header("TwoHand Properties")]
    public TwoHandType Type;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "TwoHand" : options.ClassID;
        TwoHand newRoot = (TwoHand)base.GenerateRootObject(options);
        newRoot.Clone(this, options);
        return newRoot;
    }
    public override void Clone(RootScriptObject source, RootOptions options)
    {
        base.Clone(source, options);

        if (!(source is TwoHand))
            return;

        TwoHand twoSource = (TwoHand)source;
        Type = twoSource.Type;
    }
    public override bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        slotBin = character.EquipmentSlots;

        if (slotBin[(int)EquipSlot.OFF] != null &&
            !slotBin[(int)EquipSlot.OFF].UnEquipFromCharacter(character))
        { return false; }

        if (slotBin[(int)EquipSlot.MAIN] != null &&
            !slotBin[(int)EquipSlot.MAIN].UnEquipFromCharacter(character))
        { return false; }


        if (!base.EquipToCharacter(character, slotBin, inventorySlot, (int)EquipSlot.MAIN, subSlotIndex))
            return false;

        slotBin[(int)EquipSlot.OFF] = slotBin[(int)EquipSlot.MAIN];

        base.UpdateCharacterRender(character);
        return true;
    }
}