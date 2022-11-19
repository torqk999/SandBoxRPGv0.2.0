using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SlotPanel : MonoBehaviour
{
    public DraggableButton[] Occupants;
    public RectTransform OccupantContent;

    public PlaceHolderButton[] Places;
    public RectTransform PlaceContent;

    public RectTransform MainContent;
    public void Resize(int size)
    {
        Occupants = new DraggableButton[size];
        Places = new PlaceHolderButton[size];
    }

    public void RefreshContentSize()
    {
        Vector2 newDelta = PlaceContent.sizeDelta;
        MainContent.sizeDelta = newDelta;
        OccupantContent.sizeDelta = newDelta;
    }

    public void Update()
    {
        RefreshContentSize();
    }
}
