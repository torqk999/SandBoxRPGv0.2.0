using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public List<ItemWrapper> Items;
    public int MaxCount;

    public Inventory()
    {
        Items = new List<ItemWrapper>();
    }

    /*#region EQUIPPING
    public bool EquipSelection(Character currentCharacter, int equipIndex, int inventoryIndex, bool bLeftHand)
    {
        if (inventoryIndex == -1 && equipIndex != -1)
            return AttemptEquipRemoval(currentCharacter.EquipmentSlots[equipIndex], currentCharacter, equipIndex);

        if (inventoryIndex != -1 && equipIndex == -1)
        {
            if (currentCharacter.Inventory.Items[inventoryIndex] == null || !(currentCharacter.Inventory.Items[inventoryIndex] is EquipWrapper))
                return false;

            EquipWrapper equip = (EquipWrapper)currentCharacter.Inventory.Items[inventoryIndex];

            if (equip is WearableWrapper)
            {
                WearableWrapper wear = (WearableWrapper)equip;
                int slotNumber = (int)wear.Wear.Type;

                if (slotNumber < 6)
                {
                    EquipWear(currentCharacter, slotNumber, inventoryIndex);
                    return true;
                }
                EquipRing(currentCharacter, inventoryIndex, bLeftHand);
                return true;
            }

            if (equip is TwoHandWrapper)
                return EquipTwoHand(currentCharacter, inventoryIndex);

            return EquipOneHand(currentCharacter, inventoryIndex, bLeftHand);
        }



        Debug.Log("How did you get here? >.>");
        return false;
    }
    void EquipWear(Character currentCharacter, int equipIndex, int inventoryIndex)
    {
        if (currentCharacter.EquipmentSlots[equipIndex] == null)
        {
            currentCharacter.EquipmentSlots[equipIndex] =
                (EquipWrapper)RemoveIndexFromInventory(inventoryIndex);
            return;
        }

        currentCharacter.EquipmentSlots[equipIndex] =
            (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[equipIndex], inventoryIndex);


    }
    bool EquipOneHand(Character currentCharacter, int inventoryIndex, bool bLeftHand)
    {
        int slot = (bLeftHand) ? 6 : 7;


        if (currentCharacter.EquipmentSlots[slot] == null)
        {
            currentCharacter.EquipmentSlots[slot] = (EquipWrapper)RemoveIndexFromInventory(inventoryIndex);
            return true;
        }
        if (currentCharacter.EquipmentSlots[slot] is TwoHandWrapper)
        {
            if (currentCharacter.Inventory.Items.Count == currentCharacter.Inventory.MaxCount)
                return false;

            currentCharacter.EquipmentSlots[6] = (bLeftHand) ? (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[slot], inventoryIndex) : null;
            currentCharacter.EquipmentSlots[7] = (!bLeftHand) ? (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[slot], inventoryIndex) : null;
            return true;
        }
        currentCharacter.EquipmentSlots[slot] = (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[slot], inventoryIndex);
        return true;
    }
    bool EquipTwoHand(Character currentCharacter, int inventoryIndex)
    {
        if (currentCharacter.EquipmentSlots[6] == null && currentCharacter.EquipmentSlots[7] == null)
        {
            currentCharacter.EquipmentSlots[6] = (EquipWrapper)RemoveIndexFromInventory(inventoryIndex);
            currentCharacter.EquipmentSlots[7] = currentCharacter.EquipmentSlots[6];
            return true;
        }
        if (currentCharacter.EquipmentSlots[6] != null && currentCharacter.EquipmentSlots[7] != null)
        {
            if (currentCharacter.Inventory.Items.Count == currentCharacter.Inventory.MaxCount)
                return false;
            currentCharacter.EquipmentSlots[6] =
                (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[6], inventoryIndex);
            PushItemIntoInventory(currentCharacter.EquipmentSlots[7]);
            currentCharacter.EquipmentSlots[7] = currentCharacter.EquipmentSlots[6];
            return true;
        }
        if (currentCharacter.EquipmentSlots[6] != null && currentCharacter.EquipmentSlots[7] == null)
        {
            currentCharacter.EquipmentSlots[6] =
                (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[6], inventoryIndex);
            currentCharacter.EquipmentSlots[7] = currentCharacter.EquipmentSlots[6];
            return true;
        }
        if (currentCharacter.EquipmentSlots[6] == null && currentCharacter.EquipmentSlots[7] != null)
        {
            currentCharacter.EquipmentSlots[7] =
                (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[7], inventoryIndex);
            currentCharacter.EquipmentSlots[6] = currentCharacter.EquipmentSlots[7];
            return true;
        }
        Debug.Log("How did you get here? >.>");
        return false;
    }
    void EquipRing(Character currentCharacter, int inventoryIndex, bool bLeftHand)
    {
        int slot = (bLeftHand) ? 8 : 9;

        if (currentCharacter.EquipmentSlots[slot] == null)
        {
            currentCharacter.EquipmentSlots[slot] = (EquipWrapper)RemoveIndexFromInventory(inventoryIndex);
            return;
        }

        currentCharacter.EquipmentSlots[slot] = (EquipWrapper)SwapItemSlots(currentCharacter.EquipmentSlots[slot], inventoryIndex);
    }
    bool AttemptEquipRemoval(EquipWrapper equip, Character currentCharacter, int equipIndex)
    {
        if (equip == null)
            return false;

        if (PushItemIntoInventory(equip))
        {
            if (currentCharacter.EquipmentSlots[equipIndex] is TwoHandWrapper)
            {
                currentCharacter.EquipmentSlots[6] = null;
                currentCharacter.EquipmentSlots[7] = null;
            }
            else
                currentCharacter.EquipmentSlots[equipIndex] = null;
            return true;
        }
        return false;
    }
    #endregion*/

    /*#region LOOTING
    public bool LootContainer(Character currentCharacter, int containerIndex, int inventoryIndex)
    {
        if (Items[containerIndex] is StackableWrapper)
        {
            Debug.Log("found stackable!: " + Items[containerIndex].Name);
        }

        if (currentCharacter.Inventory.PushItemIntoInventory(Items[containerIndex]))
        {
            RemoveIndexFromInventory(containerIndex);
            return true;
        }
        return false;
    }
    #endregion*/

    #region INVENTORY
    public bool PushItemIntoStack(StackableWrapper stackItem)
    {
        int stackIndex = Items.FindIndex(x =>
        x is StackableWrapper &&
        x.Name == stackItem.Name);

        if (stackIndex == -1) // Not Found
            return false;

        StackableWrapper stackTarget = (StackableWrapper)Items[stackIndex];
        if (stackTarget.Item.MaxQuantity - stackTarget.CurrentQuantity > stackItem.CurrentQuantity)
        {

        } // Move Whole Stack

        return true;
    }
    public bool PushItemIntoInventory(ItemWrapper input, int inventoryIndex = 0)
    {
        if (Items.Count >= MaxCount)
            return false;

        Items.Insert(inventoryIndex, input);
        return true;
    }
    public ItemWrapper RemoveIndexFromInventory(int inventoryIndex)
    {
        ItemWrapper output = Items[inventoryIndex];
        Items.RemoveAt(inventoryIndex);
        return output;
    }
    public ItemWrapper SwapItemSlots(ItemWrapper input, int InventoryIndex)
    {
        ItemWrapper output = Items[InventoryIndex];
        Items[InventoryIndex] = input;
        return output;
    }

    public bool TransferItem(Inventory targetInventory, int inventoryIndex, int targetIndex = 0)
    {
        if (targetInventory.PushItemIntoInventory(Items[inventoryIndex]))
        {
            Items[inventoryIndex] = null;
            return true;
        }
        return false;
    }

    #endregion
}
