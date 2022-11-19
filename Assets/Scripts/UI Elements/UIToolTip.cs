using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIToolTip : MonoBehaviour
{
    public RectTransform MyRect;
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Stats;
    public TextMeshProUGUI Flavour;

    //public DraggableButton Dragging;
    public bool NeedsRefresh;
    public bool Active;
    //public Vector2 ClickOffset;
    public Vector2 PlaceOffset;
    public Vector2 CurrentPosMouse;
    public Vector2 SizeOfRect;

    public bool UpdateText(StringBuilder title, StringBuilder stats = null, StringBuilder flavour = null)
    {
        if (title == null)
            return false;

        Active = true;
        Title.text = title.ToString();

        if (stats != null)
            Stats.text = stats.ToString();
        else
            Stats.text = string.Empty;

        if(flavour != null)
            Flavour.text = flavour.ToString();
        else
            Flavour.text = string.Empty;


        NeedsRefresh = true;
        return true;
    }

    public void DisableTip()
    {
        Active = false;
    }

    void FollowMouse()
    {

        if (Active && !MyRect.gameObject.activeSelf)
            MyRect.gameObject.SetActive(true);
        
            

        if (!Active)
        {
            if (MyRect.gameObject.activeSelf)
                MyRect.gameObject.SetActive(false);
            return;
        }

        SizeOfRect = MyRect.sizeDelta;
        PlaceOffset = MyRect.sizeDelta / 1.9f;

        if (CurrentPosMouse.x + SizeOfRect.x > Screen.width)
            PlaceOffset.x = -PlaceOffset.x;

        if (CurrentPosMouse.y + SizeOfRect.y > Screen.height)
            PlaceOffset.y = -PlaceOffset.y;

        MyRect.position = CurrentPosMouse + PlaceOffset;
    }

    void Refresh()
    {
        if (!NeedsRefresh)
            return;

        Canvas.ForceUpdateCanvases();
        MyRect.GetComponent<VerticalLayoutGroup>().enabled = false;
        MyRect.GetComponent<VerticalLayoutGroup>().enabled = true;

        NeedsRefresh = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //MyRect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentPosMouse = (Vector2)Input.mousePosition;
        FollowMouse();
        Refresh();
    }
}
