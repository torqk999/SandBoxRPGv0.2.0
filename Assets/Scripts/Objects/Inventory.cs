using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public GameState GameState;
    public UI_SlotPanel Panel;
    public ItemObject[] Items;
    //public int CurrentQuantity;
    //public int MaxCount;

    //public SelectableButton[] InventoryButtons;
    //public PlaceHolderButton[] InventoryPlaceHolders;

    public Inventory(int count = CharacterMath.PARTY_INVENTORY_MAX)
    {
        
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
    public void SetupInventory(int count, ButtonOptions options = default)
    {
        Items = new ItemObject[count];
        Panel = new UI_SlotPanel(count, );
        //InventoryButtons = new SelectableButton[count];
        //InventoryPlaceHolders = new PlaceHolderButton[count];

        options.Type = ButtonType.INVENTORY;
        options.PlaceHolder = true; 
        //options.Folder

        for (int i = 0; i < Panel.Places.Length; i++)
            Panel.Places[i] = GameState.UIman.GeneratePlaceHolder(options);

    }
    public int CurrentQuantity()
    {
        int output = 0;
        for (int i = 0; i < Items.Length; i++)
            if (Items[i] != null)
                output++;
        return output;
    }
    public bool PushItemIntoStack(Stackable stackItem)
    {
        int empty = -1;

        for(int i = 0; i < Items.Length; i++)
        {
            if (stackItem.CurrentQuantity <= 0)
                return true;

            if (Items[i] == null &&
                empty == -1)
                empty = i;

            if (!(Items[i] is Stackable))
                continue;

            Stackable stackTarget = (Stackable)Items[i];

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        Items[empty] = stackItem;
        return true;
    }
    public bool PushItemIntoInventory(ItemObject input, int inventoryIndex = 0)
    {
        if (input is Stackable)
            return PushItemIntoStack((Stackable)input);

        if (Items[inventoryIndex] == null)
        {
            Items[inventoryIndex] = input;
            return true;
        }

        int newIndex = FindClosestEmptyIndex(inventoryIndex);
        if (newIndex != -1)
        {
            Items[newIndex] = input;
            return true;
        }

        return false;
    }
    public ItemObject RemoveIndexFromInventory(int inventoryIndex)
    {
        try
        {
            ItemObject output = Items[inventoryIndex];
            Items[inventoryIndex] = null;
            return output;
        }
        catch
        {
            return null;
        }
    }
    public ItemObject SwapItems(ItemObject input, int InventoryIndex)
    {
        ItemObject output = Items[InventoryIndex];
        Items[InventoryIndex] = input;
        return output;
    }
    public int FindClosestEmptyIndex(int startIndex)
    {
        for (int i = 0; i < Math.Abs(startIndex); i++)
        {
            int sup = startIndex + i;
            int sub = startIndex - i;

            if (sup < Items.Length &&
                Items[sup] == null)
                return sup;


            if (sub > 0 &&
                Items[sub] == null)
                return sub;
        }
        return -1;
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
