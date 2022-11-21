using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PageOptions
{
    public ButtonOptions ButtonOptions;
    public bool IsPlaceHolder;
    public int Size;
    public Page PlaceHolder;
    public ListPanel TargetBin;
    
    public PageOptions(Page placeHolder)
        // Generic Page Builder
    {
        IsPlaceHolder = false;
        PlaceHolder = placeHolder;

        TargetBin = null;
        ButtonOptions = default;
        Size = 0;
    }

    public PageOptions(ButtonOptions options, int size)
        // PlaceHolder Builder
    {
        ButtonOptions = options;
        IsPlaceHolder = true;
        Size = size;

        TargetBin = null;
        PlaceHolder = null;
    }
}

[Serializable]
public class Page : MonoBehaviour
{
    public UIManager UIman;
    public RectTransform ParentContent;
    public ListPanel Occupants;
    public ListPanel PlaceHolders;
    //public bool IsPlaceHolder;
    public int MaximumSize;

    public void Setup(ButtonOptions options)
    {
        ParentContent = gameObject.GetComponent<RectTransform>();
        Resize(options.Index_Size);
        PopulatePlaceHolders(options);
    }

    void PopulatePlaceHolders(ButtonOptions options)
    {
        //PlaceHolders.List.
        Debug.Log($"count: {PlaceHolders.List.Capacity}");
        for (int i = 0; i < PlaceHolders.List.Capacity; i++)
        {
            Debug.Log($"PlaceHolder: {i + 1}");
            options.Index_Size = i;
            PlaceHolders.List[i] = UIman.GeneratePlaceHolder(options);
        }
    }

    public void Resize()
    {
        Occupants.Resize(PlaceHolders.List.Count);
    }

    public void Resize(int size)
    {
        PlaceHolders.Resize(size);
    }

    public virtual void RefreshContentSize()
    {
        if (PlaceHolders == null)
            return;

        Vector2 newDelta = PlaceHolders.PhysicalParent.sizeDelta;
        ParentContent.sizeDelta = newDelta;
        Occupants.PhysicalParent.sizeDelta = newDelta;
    }

    #region LOOTING
    public bool LootContainer(GenericContainer loot, int containerIndex, int inventoryIndex)
    {
        ItemObject lootItem = (ItemObject)((InventoryButton)loot.Inventory.Occupants.List[containerIndex]).Root;

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
        if (Occupants.List == null ||
            index < 0 ||
            index >= Occupants.List.Count)
            return;

        if (sample == null)
            return;

        RootOptions rootOptions = new RootOptions(ref UIman.GameState.ROOT_SO_INDEX);
        ItemObject newRootObject = (ItemObject)sample.GenerateRootObject(rootOptions);

        ButtonOptions buttonOptions = new ButtonOptions(newRootObject, Occupants, index);
        Occupants.List[index] = newRootObject.GenerateMyButton(buttonOptions);
    }
    public int FindEligibleIndex()
    {
        for (int i = 0; i < Occupants.List.Count; i++)
            if (Occupants.List[i] == null)
                return i;
        return -1;
    }
    public int CurrentQuantity()
    {
        int output = 0;
        for (int i = 0; i < Occupants.List.Count; i++)
            if (Occupants.List[i] != null)
                output++;
        return output;
    }
    public bool PushItemIntoStack(Stackable stackItem)
    {
        int empty = -1;

        for (int i = 0; i < Occupants.List.Count; i++)
        {
            if (Occupants.List[i] == null && empty == -1)
                empty = i; // found an empty slot in case there were no stacks to push into

            if (stackItem.CurrentQuantity <= 0)
                return true;

            if (!(((InventoryButton)Occupants.List[i]).Root is Stackable))
                continue;

            Stackable stackTarget = (Stackable)((InventoryButton)Occupants.List[i]).Root;

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        Occupants.List[empty] = stackItem.RootLogic.Button;
        return true;
    }
    public bool PushItemIntoOccupants(ItemObject input, int inventoryIndex = 0)
    {
        Debug.Log($"input: {input != null}");

        Debug.Log("Pushing into inventory...");
        if (input is Stackable)
            return PushItemIntoStack((Stackable)input);

        if (Occupants.List[inventoryIndex] == null)
        {
            Debug.Log("Slot empty! Pushing!");
            Occupants.List[inventoryIndex] = input.RootLogic.Button;
            Debug.Log($"button: {input.RootLogic.Button != null}\n place: {Occupants.List[inventoryIndex] != null}");
            input.RootLogic.Button.Occupy((PlaceHolderButton)PlaceHolders.List[inventoryIndex]);
            return true;
        }

        int newIndex = FindClosestEmptyIndex(inventoryIndex);
        if (newIndex != -1)
        {
            Occupants.List[newIndex] = input.RootLogic.Button;
            input.RootLogic.Button.Occupy((PlaceHolderButton)PlaceHolders.List[newIndex]);
            return true;
        }

        return false;
    }
    public ItemObject RemoveIndexFromOccupants(int inventoryIndex)
    {
        try
        {
            ItemObject output = (ItemObject)((InventoryButton)Occupants.List[inventoryIndex]).Root;
            Occupants.List[inventoryIndex] = null;
            return output;
        }
        catch
        {
            return null;
        }
    }
    public ItemObject SwapItems(ItemObject input, int InventoryIndex)
    {
        ItemObject output = (ItemObject)((InventoryButton)Occupants.List[InventoryIndex]).Root;
        Occupants.List[InventoryIndex] = input.RootLogic.Button;
        return output;
    }
    public int FindClosestEmptyIndex(int startIndex)
    {
        for (int i = 0; i < Math.Abs(startIndex); i++)
        {
            int sup = startIndex + i;
            int sub = startIndex - i;

            if (sup < Occupants.List.Count &&
                Occupants.List[sup] == null)
                return sup;


            if (sub > 0 &&
                Occupants.List[sub] == null)
                return sub;
        }
        return -1;
    }
    public bool TransferItem(Page targetInventory, int inventoryIndex, int targetIndex = 0)
    {
        Debug.Log("Transfering Item...");
        if (targetInventory.PushItemIntoOccupants((ItemObject)((InventoryButton)Occupants.List[inventoryIndex]).Root, targetIndex))
        {

            //Items[inventoryIndex].RootLogic.Button.Occupy(Panel.Places[targetIndex]);
            Occupants.List[inventoryIndex] = null;
            return true;
        }
        return false;
    }

    #endregion



}
