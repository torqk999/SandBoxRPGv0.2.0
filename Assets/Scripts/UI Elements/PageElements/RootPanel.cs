using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootPanel //: Panel
{
    public Page VirtualParent;
    public List<RootScriptObject> List;

    public RootPanel(int size, Page parent)
    {
        VirtualParent = parent;
        Resize(size);
    }

    public void Resize(int size)
    {
        if (List == null)
            List = new List<RootScriptObject>(size);


        List<RootScriptObject> newList = new List<RootScriptObject>(size);

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

            if (!(List[i] is Stackable))
                continue;

            Stackable stackTarget = (Stackable)List[i];

            if (stackTarget.Name != stackItem.Name)
                continue;

            int desiredQuantity = stackTarget.MaxQuantity - stackItem.CurrentQuantity;
            int movedQuantity = stackItem.CurrentQuantity < desiredQuantity ? stackItem.CurrentQuantity : desiredQuantity;
            stackTarget.CurrentQuantity += movedQuantity;
            stackItem.CurrentQuantity -= movedQuantity;
        }

        if (empty == -1)
            return false;

        List[empty] = stackItem;
        return true;
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
