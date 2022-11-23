using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public Page VirtualParent;
    public RectTransform PhysicalParent;
    public List<SelectableButton> List;
    public PlaceHolderType PlaceType;

    public void Setup(Page source)
    {
        VirtualParent = source;
        gameObject.tag = GlobalConstants.TAG_PANEL;
        PhysicalParent = gameObject.GetComponent<RectTransform>();
        Resize(VirtualParent.PlaceHolders.List.Count);
    }

    public void Resize(int size)
    {
        List<SelectableButton> newList = new List<SelectableButton>(size);

        for (int i = 0; i < newList.Capacity || i < List.Count; i++)
        {
            if (i < List.Count)
                newList.Add(List[i]);
            else
                newList.Add(null);
        }

        List = newList;
    }

    public int ReturnEmptyIndex()
    {
        for (int i = 0; i < List.Capacity; i++)
            if (List[i] == null)
                return i;

        return -1;
    }

    public bool PushItemIntoStack(Stackable stackItem)
    {
        int empty = -1;

        for (int i = 0; i < List.Count; i++)
        {
            if (List[i] == null && empty == -1)
                empty = i; // found an empty slot in case there were no stacks to push into

            if (stackItem.CurrentQuantity <= 0)
                return true;

            if (!(((InventoryButton)List[i]).Root is Stackable))
                continue;

            Stackable stackTarget = (Stackable)((InventoryButton)List[i]).Root;

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        List[empty] = stackItem.RootLogic.Button;
        return true;
    }
}
