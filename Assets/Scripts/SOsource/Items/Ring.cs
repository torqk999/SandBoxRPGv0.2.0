using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    //[Header("OneHand Properties")]
    //public int CurrentSlotIndex;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "Ring" : options.ClassID;
        Ring newRoot = (Ring)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is Ring))
            return;

        //Ring ringSource = (Ring)source;

        //CurrentSlotIndex = -1;

        
    }
    public override bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        slotBin = character.RingSlots;

        if ((subSlotIndex >= 0 ||
            subSlotIndex < CharacterMath.RING_SLOT_COUNT) &&
            !slotBin[subSlotIndex].UnEquipFromCharacter(character))
            return false;
            

        for (int i = 0; i <= CharacterMath.RING_SLOT_COUNT; i++)
        {
            if (slotBin[i] == null)
            {
                slotBin[i] = (Ring)character.Inventory.RemoveIndexFromInventory(inventorySlot);
                return true;
            }
        }

        if (!slotBin[0].UnEquipFromCharacter(character))
            return false;

        if (!base.EquipToCharacter(character, slotBin, inventorySlot, 0))// Default first index of rings
            return false; 

        return true;
    }
}
