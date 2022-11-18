using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SlotPanel : MonoBehaviour
{
    public DraggableButton[] Occupants;
    public Transform OccupantContent;

    public PlaceHolderButton[] Places;
    public Transform PlaceContent;
    
    public UI_SlotPanel(int size, Transform[] content)
    {
        Resize(size);
    }

    public void Resize(int size)
    {
        Occupants = new DraggableButton[size];
        Places = new PlaceHolderButton[size];
    }
}
