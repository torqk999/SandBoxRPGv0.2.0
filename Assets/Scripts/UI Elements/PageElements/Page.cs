using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PageOptions
{
    public ButtonOptions ButtonOptions;
    //public PlaceHolderType PlaceHolderType;
    public bool IsPlaceHolder;
    public int Size;
    public Page PlaceHolder;
    public Panel TargetBin;
    
    public PageOptions(Page placeHolder)
        // Generic Page Builder
    {
        IsPlaceHolder = false;
        PlaceHolder = placeHolder;
        //PlaceHolderType = placeHolder.PlaceHolders.PlaceType;

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
    public RootPanel OccupantRoots;
    public ButtonPanel Buttons;
    public PlaceHolderType PlaceType;
    public int MaximumSize;
    public bool Refresh;

    public void Setup(ButtonOptions options)
    {
        Debug.Log("Setup Call");

        ParentContent = gameObject.GetComponent<RectTransform>();
        Resize(options.Index_Size);
        PlaceType = options.PlaceType;
        PopulatePlaceHolders(options);
        //RefreshContentSize();
    }

    void PopulatePlaceHolders(ButtonOptions options)
    {
        for (int i = 0; i < Buttons.List.Capacity; i++)
        {
            options.Index_Size = i;
            Buttons.List[i] = UIman.GeneratePlaceHolder(options);
        }
    }
    public void PopulateLiterals(ButtonOptions options)
    {
        PlaceType = options.PlaceType;
        Clear();

        for (int i = 0; i < Buttons.List.Capacity; i++)
        {
            options.Index_Size = i;
            Buttons.List[i] = UIman.GeneratePlaceHolder(options);
        }
    }
    void AppendRoot(RootScriptObject root)
    {

    }

    public void Clear()
    {
        foreach (SelectableButton button in Buttons.List)
            Destroy(button);

        Buttons.List.Clear();
        OccupantRoots.List.Clear();
    }
    public void Add(ButtonOptions options)
    {
        OccupantRoots.List.Add(options.Root);
        Buttons.List.Add(UIman.GeneratePlaceHolder(options));
    }
    public void Remove(RootScriptObject root)
    {
        if (root == null)
            return;

        int index = root.RootLogic.Options.Index;
        Remove(index);
    }

    public void Remove(int index)
    {
        if (index < 0 ||
            index >= Buttons.List.Count)
            return;

        OccupantRoots.List.RemoveAt(index);
        Destroy(Buttons.List[index]);
        Buttons.List.RemoveAt(index);
    }

    public void Resize()
    {
        OccupantRoots.Resize(Buttons.List.Count);
    }

    public void Resize(int size)
    {
        Buttons.Resize(size);
    }

    public void RefreshContentSize()
    {
        Refresh = false;

        if (Buttons == null)
            return;

        Vector2 newDelta = Buttons.PhysicalParent.sizeDelta;
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
    public void GenerateRootIntoSlot(RootOptions rootOptions)
    {
        Debug.Log("how?");
        if (OccupantRoots.List == null ||
            rootOptions.Index < 0 ||
            rootOptions.Index >= OccupantRoots.List.Count)
            return;

        if (rootOptions.Root == null)
            return;
        Debug.Log("how?");
        //RootOptions rootOptions = new RootOptions(ref UIman.GameState.ROOT_SO_INDEX, index);
        ItemObject newRootObject = (ItemObject)rootOptions.Root.GenerateRootObject(rootOptions);
        Debug.Log("What?");
        //ButtonOptions buttonOptions = new ButtonOptions(newRootObject, PlaceHolders, index);
        Buttons.List[rootOptions.Index].Assign(newRootObject);
    }
    public int FindEligibleIndex()
    {
        for (int i = 0; i < OccupantRoots.List.Count; i++)
            if (OccupantRoots.List[i] == null)
                return i;
        return -1;
    }
    public int CurrentQuantity()
    {
        int output = 0;
        for (int i = 0; i < OccupantRoots.List.Count; i++)
            if (OccupantRoots.List[i] != null)
                output++;
        return output;
    }
    
    public bool PushItemIntoOccupants(ItemObject input, int inventoryIndex = 0)
    {
        Debug.Log($"input: {input != null}");

        Debug.Log("Pushing into inventory...");
        if (input is Stackable)
            return OccupantRoots.PushItemIntoStack((Stackable)input);

        if (OccupantRoots.List[inventoryIndex] == null)
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

            if (sup < OccupantRoots.List.Count &&
                OccupantRoots.List[sup] == null)
                return sup;


            if (sub > 0 &&
                OccupantRoots.List[sub] == null)
                return sub;
        }
        return -1;
    }
    public bool TransferItem(Page targetInventory, int sourceIndex, int targetIndex = 0)
    {
        ItemObject item = (ItemObject)OccupantRoots.List[sourceIndex];
        if (item == null)
            return false;

        if (!targetInventory.OccupantRoots.List[targetIndex].Vacate())
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
