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

    public override int EquipCharacter(Character character, int inventorySlot, int destinationIndex = 0)
    {
        int callReturn = base.EquipCharacter(character, inventorySlot);

        switch(callReturn)
        {
            default:
                return -1;

            case 0:
                Ring[] slots = character.RingSlots;
                //int sourceIndex = Array.FindIndex(slots, x => x == this);

                for (int i = 0; i <= CharacterMath.RING_SLOT_COUNT; i++)
                {
                    if (slots[i] == null)
                    {
                        slots[i] = (Ring)character.Inventory.RemoveIndexFromInventory(inventorySlot);
                        break;
                    }
                }

                slots[0].EquipCharacter(character, -1);
                slots[0] = (Ring)character.Inventory.RemoveIndexFromInventory(inventorySlot); // Default first index of rings

                // Add Abilities and Risiduals

                return 0;

            case 1:
                return 1;
        }
    }
}
