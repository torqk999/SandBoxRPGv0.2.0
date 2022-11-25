using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlaceHolderType
{
    NONE,
    CHARACTER,
    INVENTORY,
    EQUIP,
    SKILL,
    EFFECT,
    HOT_BAR
}
public class ButtonPanel : MonoBehaviour
{
    //public PlaceHolderType PlaceType;
    public Page VirtualParent;
    public RectTransform PhysicalParent;
    public List<SelectableButton> List;
    
    public void Setup(Page source)
    {
        PhysicalParent = gameObject.GetComponent<RectTransform>();
        //base.Setup(source);
        //VirtualParent = source;
        //gameObject.tag = GlobalConstants.TAG_PANEL;
        
        //Resize(VirtualParent.PlaceHolders.List.Count);
    }

    public void Resize(int size)
    {
        List<SelectableButton> newList = new List<SelectableButton>(size);

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
