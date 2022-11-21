using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListPanel : MonoBehaviour
{
    public Page VirtualParent;
    public RectTransform PhysicalParent;
    public List<SelectableButton> List;

    public void Setup(Page source)
    {
        VirtualParent = source;
        PhysicalParent = gameObject.GetComponent<RectTransform>();
        Resize(VirtualParent.PlaceHolders.List.Count);
    }
    
    public void Resize(int size)
    {
        List<SelectableButton> newList = new List<SelectableButton>(size);
        for (int i = 0; i < newList.Count && i < List.Count; i++)
            newList[i] = List[i];
        List = newList;
    }
}
