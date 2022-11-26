using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


#region MOTH BALL

/*void RaycastClick()
    {
        var pd = new PointerEventData(EventSystem.current);
        pd.position = Input.mousePosition;
        HitBuffer.Clear();
        MyRayCaster.Raycast(pd, HitBuffer);
        RootUI rootButton = null;
        Page rootPage = null;

        foreach (RaycastResult result in HitBuffer)
        {
            Debug.Log($"name: {result.gameObject.name}");

            if (result.gameObject == null)
                continue;

            if (result.gameObject.tag == GlobalConstants.TAG_BUTTON &&
                rootButton == null)
                rootButton = result.gameObject.GetComponent<RootUI>();

            if (result.gameObject.tag == GlobalConstants.TAG_PANEL &&
                rootPage == null)
                rootPage = result.gameObject.GetComponent<Page>();

        }
    }*/

#endregion

public class UIToolTip : MonoBehaviour
{
    public GameObject DragButton;
    public RectTransform TextRect;
    public Image ButtonImage;
    public RootUI TargetRoot;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Stats;
    public TextMeshProUGUI Flavour;

    public bool FirstRefreshPhaseDone;
    public bool NeedsRefresh;
    public bool TippedHovered;
    public bool ButtonActive;
    public bool ClickHeld;

    public Vector2 PlaceOffset;
    public Vector2 CurrentPosMouse;
    public Vector2 SizeOfRect;

    void ClearAllText()
    {
        Title.text = string.Empty;
        Stats.text = string.Empty;
        Flavour.text = string.Empty;
    }

    bool UpdateHover(TippedUI tipped)
    {
        ClearAllText();
        NeedsRefresh = true;
        if (tipped == null)
            return false;

        if (tipped.Title.Length == 0 &&
            tipped.Stats.Length == 0 &&
            tipped.Flavour.Length == 0)
            return false;

        Title.text = tipped.Title.ToString();

        if (tipped.Stats != null)
            Stats.text = tipped.Stats.ToString();
        else
            Stats.text = string.Empty;

        if(tipped.Flavour != null)
            Flavour.text = tipped.Flavour.ToString();
        else
            Flavour.text = string.Empty;
 
        return true;
    }

    public void ToggleTip(TippedUI tipped = null)
    {
        TippedHovered = UpdateHover(tipped);
        TargetRoot = (tipped != null) && (tipped is RootUI) ? (RootUI)tipped : null;
        Debug.Log($"TargetRoot present: {TargetRoot != null}");
    }

    public void ToggleDrag(bool toggle = true)
    {
        if (TargetRoot == null)
        {
            DragButton.SetActive(false);
            return;
        }

        DragButton.SetActive(toggle);
        if (toggle)
        {
            Debug.Log("Setting Drag sprite!");
            ButtonImage.sprite = TargetRoot.Root.sprite;
        }
        else
        {
            // drop code
        }
    }

    void FollowMouse()
    {
        SizeOfRect = TextRect.sizeDelta;
        PlaceOffset = TextRect.sizeDelta / 1.9f;

        if (CurrentPosMouse.x + SizeOfRect.x > Screen.width)
            PlaceOffset.x = -PlaceOffset.x;

        if (CurrentPosMouse.y + SizeOfRect.y > Screen.height)
            PlaceOffset.y = -PlaceOffset.y;

        TextRect.position = CurrentPosMouse + PlaceOffset;
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0) && !ClickHeld)
        {
            ClickHeld = true;
            ToggleDrag(true);
        }
        if (!Input.GetMouseButton(0) && ClickHeld)
        {
            ClickHeld = false;
            ToggleDrag(false);
        }
    }

    void CheckText()
    {
        if (TippedHovered && TextRect.localScale == Vector3.zero)
            TextRect.localScale = Vector3.one;

        if (!TippedHovered && TextRect.localScale == Vector3.one)
            TextRect.localScale = Vector3.zero;
    }

    void Refresh()
    {
        if (!NeedsRefresh)
            return;

        if (!FirstRefreshPhaseDone)
        {
            TextRect.localScale = Vector3.zero;
            Canvas.ForceUpdateCanvases();
            TextRect.GetComponent<VerticalLayoutGroup>().enabled = false;
            TextRect.GetComponent<VerticalLayoutGroup>().enabled = true;
            FirstRefreshPhaseDone = true;
            return;
        }

        TextRect.localScale = TippedHovered ? Vector3.one : Vector3.zero;
        FirstRefreshPhaseDone = false;
        NeedsRefresh = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        ToggleTip();
        //HitBuffer = new List<RaycastResult>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentPosMouse = (Vector2)Input.mousePosition;
        FollowMouse();
        //CheckClick();
        CheckText();
        Refresh();
    }
}
