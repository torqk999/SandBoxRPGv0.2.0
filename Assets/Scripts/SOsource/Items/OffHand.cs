using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OffHandType
{
    PARRY,
    TOTEM,
    RELIC,
    TORCH
}

[CreateAssetMenu(fileName = "OffHand", menuName = "ScriptableObjects/Equipment/OffHand")]
public class OffHand : Hand
{
    [Header("OffHand Properties")]
    public OffHandType Type;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "OffHand" : options.ClassID;
        OffHand newRoot = (OffHand)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is OffHand))
            return;

        OffHand offSource = (OffHand)source;
        Type = offSource.Type;
    }

    public override bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        slotBin = character.EquipmentSlots;

        if (slotBin[(int)EquipSlot.OFF] != null &&
            !slotBin[(int)EquipSlot.OFF].UnEquipFromCharacter(character))
        {
            return false; // failed to remove the piece currently occupying the slot
        }

        if (!base.EquipToCharacter(character, slotBin, inventorySlot, (int)EquipSlot.OFF, subSlotIndex))
            return false;

        base.UpdateCharacterRender(character);
        return true;
    }
}
