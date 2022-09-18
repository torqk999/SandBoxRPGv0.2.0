using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public List<ItemWrapper> Items;
    public int MaxCount;

    public Inventory(int count = CharacterMath.PARTY_INVENTORY_MAX)
    {
        Items = new List<ItemWrapper>();
    }
    #region LOOTING
    public bool LootContainer(GenericContainer loot, int containerIndex, int inventoryIndex)
    {
        if (loot.Inventory.Items[containerIndex] is StackableWrapper &&
            PushItemIntoStack((StackableWrapper)loot.Inventory.Items[containerIndex]))
        {
            loot.Inventory.RemoveIndexFromInventory(containerIndex);
            return true;
            //Debug.Log("found stackable!: " + Inventory.Items[containerIndex].Name);
        }

        if (PushItemIntoInventory(loot.Inventory.Items[containerIndex]))
        {
            loot.Inventory.RemoveIndexFromInventory(containerIndex);
            return true;
        }
        return false;
    }
    #endregion

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
