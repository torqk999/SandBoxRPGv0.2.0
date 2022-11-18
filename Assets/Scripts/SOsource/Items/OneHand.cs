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

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "OneHand" : options.ClassID;
        OneHand newRoot = (OneHand)base.GenerateRootObject(options);
        newRoot.Clone(this, options);
        return newRoot;
    }
    public override void Clone(RootScriptObject source, RootOptions options)
    {
        base.Clone(source, options);

        if (!(source is OneHand))
            return;

        OneHand oneSource = (OneHand)source;
        Type = oneSource.Type;
    }
    public override bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        slotBin = character.EquipmentSlots;

        if (slotBin[(int)EquipSlot.MAIN] != null &&
            !slotBin[(int)EquipSlot.MAIN].UnEquipFromCharacter(character))
        {
            return false; // failed to remove the piece currently occupying the slot
        }

        if (!base.EquipToCharacter(character, slotBin, inventorySlot, (int)EquipSlot.MAIN, subSlotIndex))
            return false;
        
        base.UpdateCharacterRender(character);
        return true;
    }
}