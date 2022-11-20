using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrayPanel : SlotPanel
{
    public SelectableButton[] Places;

    public ArrayPanel(int size, RectTransform content)
    {
        Resize(size);
        PlaceContent = content;
    }
    public override void Resize(int size)
    {
        Places = new SelectableButton[size];
    }

    public void BuildPlaceHolders(int size, RectTransform content)
    {
        Resize(size);
    }
}
