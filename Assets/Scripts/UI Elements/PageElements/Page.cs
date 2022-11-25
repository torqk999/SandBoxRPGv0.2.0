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
    public Panel TargetBin;
    
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
    public RootPanel Occupants;
    public ButtonPanel PlaceHolders;
    //public bool IsPlaceHolder;
    public int MaximumSize;
    public bool Refresh;

    public void Setup(ButtonOptions options)
    {
        Debug.Log("Setup Call");

        ParentContent = gameObject.GetComponent<RectTransform>();
        Resize(options.Index_Size);
        PopulatePlaceHolders(options);
        //RefreshContentSize();
    }

    void PopulatePlaceHolders(ButtonOptions options)
    {
        for (int i = 0; i < PlaceHolders.List.Capacity; i++)
        {
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

    public void RefreshContentSize()
    {
        Refresh = false;

        if (PlaceHolders == null)
            return;

        Vector2 newDelta = PlaceHolders.PhysicalParent.sizeDelta;
        Debug.Log($"SizeDelta: {newDelta}");
        ParentContent.sizeDelta = newDelta;
    }

    #region LOOTING
    /*public bool LootContainer(GenericContainer loot, int containerIndex, int inventoryIndex)
    {
        ItemObject lootItem = (ItemObject)((InventoryButton)loot.Inventory.Occupants.List[containerIndex]).Root;

        if (lootItem is Stackable &&
            Occupants.PushItemIntoStack((Stackable)lootItem))
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
    }*/


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

        RootOptions rootOptions = new RootOptions(ref UIman.GameState.ROOT_SO_INDEX, index);
        ItemObject newRootObject = (ItemObject)sample.GenerateRootObject(rootOptions);

        //ButtonOptions buttonOptions = new ButtonOptions(newRootObject, PlaceHolders, index);
        PlaceHolders.List[index].Assign(newRootObject);
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
    
    public bool PushItemIntoOccupants(ItemObject input, int inventoryIndex = 0)
    {
        Debug.Log($"input: {input != null}");

        Debug.Log("Pushing into inventory...");
        if (input is Stackable)
            return Occupants.PushItemIntoStack((Stackable)input);

        if (Occupants.List[inventoryIndex] == null)
        {
            input.Occupy(this, inventoryIndex);
            return true;
        }

        int newIndex = FindClosestEmptyIndex(inventoryIndex);
        if (newIndex != -1)
        {
            input.Occupy(this, inventoryIndex);
            return true;
        }

        return false;
    }
    /*public ItemObject RemoveIndexFromOccupants(int inventoryIndex)
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
    }*/
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
    public bool TransferItem(Page targetInventory, int sourceIndex, int targetIndex = 0)
    {
        ItemObject item = (ItemObject)Occupants.List[sourceIndex];
        if (item == null)
            return false;

        if (!targetInventory.Occupants.List[targetIndex].Vacate())
            return false;

        item.Vacate();
        item.Occupy(targetInventory, targetIndex);
        return true;
    }

    public void Update()
    {
        if (Refresh)
            RefreshContentSize();
    }

    #endregion
}
