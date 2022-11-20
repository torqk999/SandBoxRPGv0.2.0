using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotPanel
{
    public SelectableButton[] Places;
    public RectTransform PlaceContent;

    public SlotPanel(int size, RectTransform content)
    {
        Resize(size);
        PlaceContent = content;
    }
    public void Resize(int size)
    {
        Places = new SelectableButton[size];
    }

    public void BuildPlaceHolders(int size, RectTransform content)
    {
        Resize(size);
    }
}
