using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    //[Header("OneHand Properties")]
    //public int CurrentSlotIndex;

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        Ring newOneHand = (Ring)CreateInstance("Ring");
        newOneHand.CloneItem(this, equipId, inject);
        return newOneHand;
    }
    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is Ring))
            return;

        //Ring ringSource = (Ring)source;

        //CurrentSlotIndex = -1;

        base.CloneItem(source, equipId, inject);
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
                slotBin[0] = (Ring)character.Inventory.RemoveIndexFromInventory(inventorySlot); // Default first index of rings
        return true;
    }
}
