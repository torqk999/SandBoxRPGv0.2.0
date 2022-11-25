using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    public Page VirtualParent;
    //public RectTransform PhysicalParent;
    //public List<SelectableButton> List;
    //public PlaceHolderType PlaceType;

    public virtual void Setup(Page source)
    {
        VirtualParent = source;
        gameObject.tag = GlobalConstants.TAG_PANEL;
        //PhysicalParent = gameObject.GetComponent<RectTransform>();
        Resize(VirtualParent.PlaceHolders.List.Count);
    }

    public virtual void Resize(int size)
    {
        /*List<SelectableButton> newList = new List<SelectableButton>(size);

        for (int i = 0; i < newList.Capacity || i < List.Count; i++)
        {
            if (i < List.Count)
                newList.Add(List[i]);
            else
                newList.Add(null);
        }

        List = newList;*/
    }

    public virtual int ReturnEmptyIndex()
    {
        /*for (int i = 0; i < List.Capacity; i++)
            if (List[i] == null)
                return i;*/

        return -1;
    }

    


}
