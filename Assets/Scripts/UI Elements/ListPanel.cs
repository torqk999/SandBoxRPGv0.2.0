using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListPanel : SlotPanel
{
    public List<SelectableButton> Places;

    public ListPanel(int size, RectTransform content)
    {
        Resize(size);
        PlaceContent = content;
    }
    public override void Resize(int size)
    {
        Places = new List<SelectableButton>(size);
    }
}
