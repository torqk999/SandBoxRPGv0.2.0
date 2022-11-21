using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListPanel
{
    public RectTransform PlaceContent;
    public List<SelectableButton> Places;

    public ListPanel(PageOptions options)
    {
        Setup(options);
    }

    public void Setup(PageOptions options)
    {
        PlaceContent = options.TargetContent;
        Resize(options.Size);
    }

    public void Resize(int size)
    {
        List<SelectableButton> newList = new List<SelectableButton>(size);
        for (int i = 0; i < newList.Count && i < Places.Count; i++)
            newList[i] = Places[i];
        Places = newList;
    }
}
