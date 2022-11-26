using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIToolTip : MonoBehaviour
{
    public GameObject DragButton;
    public RectTransform TextRect;
    public RectTransform ContainerRect;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Stats;
    public TextMeshProUGUI Flavour;

    public GraphicRaycaster MyRayCaster;
    public List<RaycastResult> HitBuffer;

    public bool NeedsRefresh;
    public bool TextActive;
    public bool ButtonActive;

    public Vector2 PlaceOffset;
    public Vector2 CurrentPosMouse;
    public Vector2 SizeOfRect;

    bool UpdateText(StringBuilder title, StringBuilder stats = null, StringBuilder flavour = null)
    {
        if (title == null)
            return false;

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

    public void ToggleTip(bool toggle,
        StringBuilder title = null,
        StringBuilder stats = null,
        StringBuilder flavour = null)
    {
        TextActive = toggle;

        if (TextActive)
            UpdateText(title, stats, flavour);
    }

    public void ToggleDrag()
    {
        
    }

    void FollowMouse()
    {
        if (TextActive && !ContainerRect.gameObject.activeSelf)
            ContainerRect.gameObject.SetActive(true);
        
        if (!TextActive)
        {
            if (ContainerRect.gameObject.activeSelf)
                ContainerRect.gameObject.SetActive(false);
            return;
        }

        SizeOfRect = ContainerRect.sizeDelta;
        PlaceOffset = ContainerRect.sizeDelta / 1.9f;

        if (CurrentPosMouse.x + SizeOfRect.x > Screen.width)
            PlaceOffset.x = -PlaceOffset.x;

        if (CurrentPosMouse.y + SizeOfRect.y > Screen.height)
            PlaceOffset.y = -PlaceOffset.y;

        ContainerRect.position = CurrentPosMouse + PlaceOffset;
    }

    void Refresh()
    {
        if (!NeedsRefresh)
            return;

        Canvas.ForceUpdateCanvases();
        TextRect.GetComponent<VerticalLayoutGroup>().enabled = false;
        TextRect.GetComponent<VerticalLayoutGroup>().enabled = true;

        NeedsRefresh = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ContainerRect = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentPosMouse = (Vector2)Input.mousePosition;
        FollowMouse();
        Refresh();
    }
}
