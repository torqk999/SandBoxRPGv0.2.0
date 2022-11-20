using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotPage
{
    public GameState GameState;
    public ArrayPanel Occupants;
    public ArrayPanel PlaceHolders;
    public RectTransform ParentContent;

    public void ResetContent(ArrayPanel placeHolders, RectTransform parentContent, RectTransform occupantContent)
    {
        ParentContent = parentContent;
        PlaceHolders = placeHolders;
        int size = PlaceHolders.Places.Length;
        //Roots = new RootScriptObject[size];
        Occupants = new ArrayPanel(size, occupantContent);
    }

    public void RefreshContentSize()
    {
        if (PlaceHolders == null)
            return;

        Vector2 newDelta = PlaceHolders.PlaceContent.sizeDelta;
        ParentContent.sizeDelta = newDelta;
        Occupants.PlaceContent.sizeDelta = newDelta;
    }

    #region LOOTING
    public bool LootContainer(GenericContainer loot, int containerIndex, int inventoryIndex)
    {
        ItemObject lootItem = (ItemObject)((InventoryButton)loot.Inventory.Occupants.Places[containerIndex]).Root;

        if (lootItem is Stackable &&
            PushItemIntoStack((Stackable)lootItem))
        {
            loot.Inventory.RemoveIndexFromOccupants(containerIndex);
            return true;
            //Debug.Log("found stackable!: " + Inventory.Items[containerIndex].Name);
        }

        if (PushItemIntoOccupants(lootItem))
        {
            loot.Inventory.RemoveIndexFromOccupants(containerIndex);
            return true;
        }
        return false;
    }
    #endregion

    #region INVENTORY
    public void GenerateItem(ItemObject sample, int index)
    {
        if (Occupants.Places == null ||
            index < 0 ||
            index >= Occupants.Places.Length)
            return;

        if (sample == null)
            return;

        RootOptions rootOptions = new RootOptions(ref GameState.ROOT_SO_INDEX);
        ItemObject newRootObject = (ItemObject)sample.GenerateRootObject(rootOptions);
        
        ButtonOptions buttonOptions = new ButtonOptions(newRootObject, this, index);
        Occupants.Places[index] = newRootObject.GenerateMyButton(buttonOptions);
        //buttonOptions.ButtonType = ButtonType.DRAG;
        //buttonOptions.PlaceType = PlaceHolderType.INVENTORY;

        //Panel.Roots[index].GenerateMyButton();
    }
    public void SetupPage(GameState state, int count, string pageName)
    {
        GameState = state;
        //Items = new ItemObject[count];
        Panel.ResetContent(GameState.UIman.InventoryPlaceHolders, true);

        ButtonOptions options = new ButtonOptions(Panel);

        //options.ButtonType = ButtonType.PLACE;
        options.PlaceType = PlaceHolderType.INVENTORY;

        for (int i = 0; i < Panel.Places.Length; i++)
        {
            options.Index = i;
            Panel.Places[i] = GameState.UIman.GeneratePlaceHolder(options);
        }
    }
    public int FindEligibleIndex()
    {
        for (int i = 0; i < Occupants.Places.Length; i++)
            if (Occupants.Places[i] == null)
                return i;
        return -1;
    }
    public int CurrentQuantity()
    {
        int output = 0;
        for (int i = 0; i < Panel.Roots.Length; i++)
            if (Panel.Roots[i] != null)
                output++;
        return output;
    }
    public bool PushItemIntoStack(Stackable stackItem)
    {
        int empty = -1;

        for (int i = 0; i < Panel.Roots.Length; i++)
        {
            if (stackItem.CurrentQuantity <= 0)
                return true;

            if (Panel.Roots[i] == null &&
                empty == -1)
                empty = i;

            if (!(Panel.Roots[i] is Stackable))
                continue;

            Stackable stackTarget = (Stackable)Panel.Roots[i];

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        Panel.Roots[empty] = stackItem;
        return true;
    }
    public bool PushItemIntoOccupants(ItemObject input, int inventoryIndex = 0)
    {
        Debug.Log($"input: {input != null}");

        Debug.Log("Pushing into inventory...");
        if (input is Stackable)
            return PushItemIntoStack((Stackable)input);

        if (Panel.Roots[inventoryIndex] == null)
        {
            Debug.Log("Slot empty! Pushing!");
            Panel.Roots[inventoryIndex] = input;
            Debug.Log($"button: {input.RootLogic.Button != null}\n place: {Panel.Places[inventoryIndex] != null}");
            input.RootLogic.Button.Occupy(Panel.Places[inventoryIndex]);
            return true;
        }

        int newIndex = FindClosestEmptyIndex(inventoryIndex);
        if (newIndex != -1)
        {
            Panel.Roots[newIndex] = input;
            input.RootLogic.Button.Occupy(Panel.Places[newIndex]);
            return true;
        }

        return false;
    }
    public ItemObject RemoveIndexFromOccupants(int inventoryIndex)
    {
        try
        {
            ItemObject output = (ItemObject)Panel.Roots[inventoryIndex];
            Panel.Roots[inventoryIndex] = null;
            return output;
        }
        catch
        {
            return null;
        }
    }
    public ItemObject SwapItems(ItemObject input, int InventoryIndex)
    {
        ItemObject output = (ItemObject)Panel.Roots[InventoryIndex];
        Panel.Roots[InventoryIndex] = input;
        return output;
    }
    public int FindClosestEmptyIndex(int startIndex)
    {
        for (int i = 0; i < Math.Abs(startIndex); i++)
        {
            int sup = startIndex + i;
            int sub = startIndex - i;

            if (sup < Panel.Roots.Length &&
                Panel.Roots[sup] == null)
                return sup;


            if (sub > 0 &&
                Panel.Roots[sub] == null)
                return sub;
        }
        return -1;
    }
    public bool TransferItem(SlotPage targetInventory, int inventoryIndex, int targetIndex = 0)
    {
        Debug.Log("Transfering Item...");
        if (targetInventory.PushItemIntoOccupants((ItemObject)Panel.Roots[inventoryIndex], targetIndex))
        {

            //Items[inventoryIndex].RootLogic.Button.Occupy(Panel.Places[targetIndex]);
            Panel.Roots[inventoryIndex] = null;
            return true;
        }
        return false;
    }

    #endregion
}
