using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RootPanel //: Panel
{
    public Page VirtualParent;
    public PlaceHolderType PlaceType;
    public List<RootScriptObject> List;

    public RootPanel(int size, Page parent = null)
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

    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
