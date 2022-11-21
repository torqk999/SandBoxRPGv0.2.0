using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PageOptions
{
    public ButtonOptions ButtonOptions;
    public bool IsPlaceHolder;
    public int Size;
    //public RectTransform ParentContent;
    public RectTransform TargetContent;
    public ListPanel TargetBin;
    
    public PageOptions(ButtonOptions options,
        //bool placeHolder,
        //int size,
        //RectTransform parentContent,
        RectTransform targetContent,
        ListPanel targetbin)
        // Generic Page Builder
    {
        ButtonOptions = options;
        IsPlaceHolder = false;
        Size = 0; // un-used
        //ParentContent = parentContent;
        TargetContent = targetContent;
        TargetBin = targetbin;
    }

    public PageOptions(ButtonOptions options, int size)
        // PlaceHolder Builder
    {
        ButtonOptions = options;
        IsPlaceHolder = true;
        Size = size;

        // Expected to not be used!!!
        //ParentContent = null;
        TargetContent = null;
        TargetBin = null;
    }
}

[Serializable]
public class Page : MonoBehaviour
{
    public UIManager UIman;
    public RectTransform ParentContent;
    public ListPanel Occupants;
    public ListPanel PlaceHolders;
    //bool IsPlaceHolder;

    public void Setup(PageOptions options)
    {
        ParentContent = gameObject.GetComponent<RectTransform>();
        //IsPlaceHolder = options.IsPlaceHolder;
        if (!options.IsPlaceHolder)
        {
            Occupants = new ListPanel(options);
            return;
        }
        PlaceHolders.Setup(options);
        PopulatePlaceHolders(options.ButtonOptions);
    }

    void PopulatePlaceHolders(ButtonOptions options)
    {
        for (int i = 0; i < PlaceHolders.Places.Count; i++)
        {
            options.Index = i;
            PlaceHolders.Places[i] = UIman.GeneratePlaceHolder(options);
        }
    }

    public void Resize(int size)
    {
        Occupants.Resize(size);
        PlaceHolders.Resize(size);
    }

    public void Retarget(PageOptions options)
    {
        if (options.IsPlaceHolder)
        {
            PlaceHolders = options.TargetBin;
        }
        else
        {
            Occupants = options.TargetBin;
        }
    }

    public virtual void RefreshContentSize()
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
            index >= Occupants.Places.Count)
            return;

        if (sample == null)
            return;

        RootOptions rootOptions = new RootOptions(ref UIman.GameState.ROOT_SO_INDEX);
        ItemObject newRootObject = (ItemObject)sample.GenerateRootObject(rootOptions);

        ButtonOptions buttonOptions = new ButtonOptions(newRootObject, this, index);
        Occupants.Places[index] = newRootObject.GenerateMyButton(buttonOptions);
    }
    public int FindEligibleIndex()
    {
        for (int i = 0; i < Occupants.Places.Count; i++)
            if (Occupants.Places[i] == null)
                return i;
        return -1;
    }
    public int CurrentQuantity()
    {
        int output = 0;
        for (int i = 0; i < Occupants.Places.Count; i++)
            if (Occupants.Places[i] != null)
                output++;
        return output;
    }
    public bool PushItemIntoStack(Stackable stackItem)
    {
        int empty = -1;

        for (int i = 0; i < Occupants.Places.Count; i++)
        {
            if (Occupants.Places[i] == null && empty == -1)
                empty = i; // found an empty slot in case there were no stacks to push into

            if (stackItem.CurrentQuantity <= 0)
                return true;

            if (!(((InventoryButton)Occupants.Places[i]).Root is Stackable))
                continue;

            Stackable stackTarget = (Stackable)((InventoryButton)Occupants.Places[i]).Root;

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        Occupants.Places[empty] = stackItem.RootLogic.Button;
        return true;
    }
    public bool PushItemIntoOccupants(ItemObject input, int inventoryIndex = 0)
    {
        Debug.Log($"input: {input != null}");

        Debug.Log("Pushing into inventory...");
        if (input is Stackable)
            return PushItemIntoStack((Stackable)input);

        if (Occupants.Places[inventoryIndex] == null)
        {
            Debug.Log("Slot empty! Pushing!");
            Occupants.Places[inventoryIndex] = input.RootLogic.Button;
            Debug.Log($"button: {input.RootLogic.Button != null}\n place: {Occupants.Places[inventoryIndex] != null}");
            input.RootLogic.Button.Occupy((PlaceHolderButton)PlaceHolders.Places[inventoryIndex]);
            return true;
        }

        int newIndex = FindClosestEmptyIndex(inventoryIndex);
        if (newIndex != -1)
        {
            Occupants.Places[newIndex] = input.RootLogic.Button;
            input.RootLogic.Button.Occupy((PlaceHolderButton)PlaceHolders.Places[newIndex]);
            return true;
        }

        return false;
    }
    public ItemObject RemoveIndexFromOccupants(int inventoryIndex)
    {
        try
        {
            ItemObject output = (ItemObject)((InventoryButton)Occupants.Places[inventoryIndex]).Root;
            Occupants.Places[inventoryIndex] = null;
            return output;
        }
        catch
        {
            return null;
        }
    }
    public ItemObject SwapItems(ItemObject input, int InventoryIndex)
    {
        ItemObject output = (ItemObject)((InventoryButton)Occupants.Places[InventoryIndex]).Root;
        Occupants.Places[InventoryIndex] = input.RootLogic.Button;
        return output;
    }
    public int FindClosestEmptyIndex(int startIndex)
    {
        for (int i = 0; i < Math.Abs(startIndex); i++)
        {
            int sup = startIndex + i;
            int sub = startIndex - i;

            if (sup < Occupants.Places.Count &&
                Occupants.Places[sup] == null)
                return sup;


            if (sub > 0 &&
                Occupants.Places[sub] == null)
                return sub;
        }
        return -1;
    }
    public bool TransferItem(Page targetInventory, int inventoryIndex, int targetIndex = 0)
    {
        Debug.Log("Transfering Item...");
        if (targetInventory.PushItemIntoOccupants((ItemObject)((InventoryButton)Occupants.Places[inventoryIndex]).Root, targetIndex))
        {

            //Items[inventoryIndex].RootLogic.Button.Occupy(Panel.Places[targetIndex]);
            Occupants.Places[inventoryIndex] = null;
            return true;
        }
        return false;
    }

    #endregion



}
