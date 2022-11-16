using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public GameState GameState;
    public List<ItemObject> Items;
    public int MaxCount;

    public Inventory(int count = CharacterMath.PARTY_INVENTORY_MAX)
    {
        Items = new List<ItemObject>();
    }
    #region LOOTING
    public bool LootContainer(GenericContainer loot, int containerIndex, int inventoryIndex)
    {
        if (loot.Inventory.Items[containerIndex] is Stackable &&
            PushItemIntoStack((Stackable)loot.Inventory.Items[containerIndex]))
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
    public bool PushItemIntoStack(Stackable stackItem)
    {
        int stackIndex = Items.FindIndex(x =>
        x is Stackable &&
        x.Name == stackItem.Name);

        if (stackIndex == -1) // Not Found
            return false;

        Stackable stackTarget = (Stackable)Items[stackIndex];
        if (stackTarget.MaxQuantity - stackTarget.CurrentQuantity > stackItem.CurrentQuantity)
        {

        } // Move Whole Stack

        return true;
    }
    public bool PushItemIntoInventory(ItemObject input, int inventoryIndex = 0)
    {
        if (Items.Count >= MaxCount)
            return false;

        Items.Insert(inventoryIndex, input);
        return true;
    }
    public ItemObject RemoveIndexFromInventory(int inventoryIndex)
    {
        ItemObject output = Items[inventoryIndex];
        Items.RemoveAt(inventoryIndex);
        return output;
    }
    public ItemObject SwapItemSlots(ItemObject input, int InventoryIndex)
    {
        ItemObject output = Items[InventoryIndex];
        Items[InventoryIndex] = input;
        return output;
    }
    public bool TransferItem(Inventory targetInventory, int inventoryIndex, int targetIndex = 0)
    {
        if (targetInventory.PushItemIntoInventory(Items[inventoryIndex], targetIndex))
        {
            Items[inventoryIndex] = null;
            return true;
        }
        return false;
    }

    #endregion
}
