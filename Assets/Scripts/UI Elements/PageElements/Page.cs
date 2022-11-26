using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PageOptions
{
    public UI_Options ButtonOptions;
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

    public PageOptions(UI_Options options, int size)
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
    public List<RootScriptObject> OccupantRoots;
    public ButtonPanel Buttons;
    public PlaceHolderType PlaceType;
    public int MaximumSize;
    public bool Refresh;

    public void Setup(UI_Options options)
    {
        Debug.Log("Setup Call");

        ParentContent = gameObject.GetComponent<RectTransform>();
        Resize(options.Index); // Temporarily used as list size...
        PlaceType = options.PlaceType;
        PopulatePlaceHolders(options);
        //RefreshContentSize();
    }

    void PopulatePlaceHolders(UI_Options options)
    {
        for (int i = 0; i < Buttons.List.Capacity; i++)
        {
            options.Index = i;
            Buttons.List[i] = UIman.GeneratePlaceHolder(options);
        }
    }
    public void PopulateLiterals(UI_Options options)
    {
        PlaceType = options.PlaceType;
        Clear();

        for (int i = 0; i < options.Page.OccupantRoots.Count; i++)
        {
            options.Root = options.Page.OccupantRoots[i];
            options.Index = i;
            options.Page.Add(options);
        }
    }

    public void Clear()
    {
        foreach (SelectableUI button in Buttons.List)
            Destroy(button);

        Buttons.List.Clear();
        OccupantRoots.Clear();
    }
    public void Add(UI_Options options)
    {
        OccupantRoots.Add(options.Root);
        Buttons.List.Add(UIman.GenerateLiteral(options));
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

        OccupantRoots.RemoveAt(index);
        Destroy(Buttons.List[index]);
        Buttons.List.RemoveAt(index);
    }

    public void Resize(int size)
    {
        Buttons.Resize(size);

        /*List<RootScriptObject> newList = new List<RootScriptObject>(size);

        for (int i = 0; i < newList.Capacity || i < OccupantRoots.Count; i++)
        {
            if (i < OccupantRoots.Count)
                newList.Add(OccupantRoots[i]);
            else
                newList.Add(null);
        }

        OccupantRoots = newList;*/
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
        if (OccupantRoots == null ||
            rootOptions.Index < 0 ||
            rootOptions.Index >= OccupantRoots.Count)
            return;

        if (rootOptions.Source == null)
            return;
        Debug.Log("Generating root into slot...");
        //RootOptions rootOptions = new RootOptions(ref UIman.GameState.ROOT_SO_INDEX, index);
        ItemObject newRootObject = (ItemObject)rootOptions.Source.GenerateRootObject(rootOptions);
        Debug.Log("Root generated at slot!");
        //ButtonOptions buttonOptions = new ButtonOptions(newRootObject, PlaceHolders, index);
        Buttons.List[rootOptions.Index].Assign(newRootObject);
    }
    public int FindEligibleIndex()
    {
        for (int i = 0; i < OccupantRoots.Count; i++)
            if (OccupantRoots[i] == null)
                return i;
        return -1;
    }
    public int CurrentQuantity()
    {
        int output = 0;
        for (int i = 0; i < OccupantRoots.Count; i++)
            if (OccupantRoots[i] != null)
                output++;
        return output;
    }
    
    public bool PushItemIntoOccupants(ItemObject input, int inventoryIndex = 0)
    {
        Debug.Log($"input: {input != null}");

        Debug.Log("Pushing into inventory...");
        if (input is Stackable)
            return PushItemIntoStack((Stackable)input);

        if (OccupantRoots[inventoryIndex] == null)
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

    public bool PushItemIntoStack(Stackable stackItem)
    {
        int empty = -1;

        for (int i = 0; i < OccupantRoots.Count; i++)
        {
            if (OccupantRoots[i] == null && empty == -1)
                empty = i; // found an empty slot in case there were no stacks to push into

            if (stackItem.CurrentQuantity <= 0)
                return true;

            if (!(OccupantRoots[i] is Stackable))
                continue;

            Stackable stackTarget = (Stackable)OccupantRoots[i];

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        OccupantRoots[empty] = stackItem;
        return true;
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

            if (sup < OccupantRoots.Count &&
                OccupantRoots[sup] == null)
                return sup;


            if (sub > 0 &&
                OccupantRoots[sub] == null)
                return sub;
        }
        return -1;
    }
    public bool TransferItem(Page targetInventory, int sourceIndex, int targetIndex = 0)
    {
        ItemObject item = (ItemObject)OccupantRoots[sourceIndex];
        if (item == null)
            return false;

        if (!targetInventory.OccupantRoots[targetIndex].Vacate())
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
