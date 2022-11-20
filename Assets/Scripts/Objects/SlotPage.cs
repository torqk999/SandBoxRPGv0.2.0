using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotPage
{
    public SlotPanel Occupants;
    public SlotPanel PlaceHolders;
    public RootScriptObject[] Roots;
    public RectTransform ParentContent;
    //public RectTransform MyContent;

    public void ResetContent(SlotPanel placeHolders, RectTransform parentContent, RectTransform occupantContent)
    {
        ParentContent = parentContent;
        PlaceHolders = placeHolders;
        int size = PlaceHolders.Places.Length;
        Roots = new RootScriptObject[size];
        Occupants = new SlotPanel(size, occupantContent);
    }

    public void RefreshContentSize()
    {
        if (PlaceHolders == null)
            return;

        Vector2 newDelta = PlaceHolders.PlaceContent.sizeDelta;
        ParentContent.sizeDelta = newDelta;
        Occupants.PlaceContent.sizeDelta = newDelta;
    }

    /*public void Update()
    {
        RefreshContentSize();
    }*/
}
