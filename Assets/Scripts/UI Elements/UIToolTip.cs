using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


#region MOTH BALL



#endregion

public class UIToolTip : MonoBehaviour
{
    public Canvas ToolTipCanvas;
    public GameObject DragButton;
    public RectTransform ContainerRect;
    public RectTransform TextRect;
    public Image ButtonImage;
    public RootUI TargetRoot;
    public RootUI StoredRoot;
    public Page TargetPage;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Stats;
    public TextMeshProUGUI Flavour;

    public GraphicRaycaster InGameRayCaster;
    public GraphicRaycaster HUDrayCaster;
    public List<RaycastResult> HitBuffer;
    //public bool FirstRefreshPhaseDone;
    //public bool SecondRefreshPhaseDone;
    public bool[] RefreshPhaseDone;
    public bool NeedsRefresh;
    public bool TippedHovered;
    public bool ButtonActive;
    public bool ClickHeld;
    public bool JustFollow;

    public Vector2 PlaceOffset;
    public Vector2 ScreenOffset;
    public Vector2 CurrentPosMouse;
    public Vector2 ContainerOffset;
    public Vector2 SizeOfRect;
    public Vector2 PixelOffset;

    bool FoundRoot = false;
    bool FoundPage = false;


    void ClearAllText()
    {
        Title.text = string.Empty;
        Stats.text = string.Empty;
        Flavour.text = string.Empty;
    }

    bool UpdateHover(TippedUI tipped)
    {
        //Debug.Log("Updating Hover!");

        ClearAllText();
        ToolTipCanvas.sortingOrder = 1;
        Canvas.ForceUpdateCanvases();
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

        if (tipped.Flavour != null)
            Flavour.text = tipped.Flavour.ToString();
        else
            Flavour.text = string.Empty;

        return true;
    }

    void ToggleTip(TippedUI tipped = null)
    {
        TippedHovered = UpdateHover(tipped);
        TargetRoot = (tipped != null) && (tipped is RootUI) ? (RootUI)tipped : null;
        Debug.Log($"TargetRoot present: {TargetRoot != null}");
    }

    void ToggleDrag(bool toggle = true)
    {
        StoredRoot = toggle ? TargetRoot : StoredRoot;
        DragButton.SetActive(toggle && StoredRoot != null && StoredRoot.Root != null);

        if (toggle && StoredRoot != null && StoredRoot.Root != null)
        {
            Debug.Log("Setting Drag sprite!");
            ButtonImage.sprite = StoredRoot.Root.sprite;
        }
        if (!toggle)
        {
            Debug.Log("RePositionRoot!");
            if (StoredRoot != null)
                StoredRoot.Root.RePositionRoot(TargetPage, TargetRoot);
        }
        Debug.Log("Hello?");
    }

    void FollowMouse()
    {
        SizeOfRect = ContainerRect.sizeDelta;

        //ScreenOffset.x = Screen.width / 2;
        //ScreenOffset.y = Screen.height / 2;
        //PlaceOffset = (CurrentPosMouse - ScreenOffset) * 1.4f;
        
        ContainerOffset = SizeOfRect / 2f;// + PixelOffset;

        if (CurrentPosMouse.x + SizeOfRect.x > Screen.width)
            ContainerOffset.x = -ContainerOffset.x;

        if (CurrentPosMouse.y + SizeOfRect.y > Screen.height)
            ContainerOffset.y = -ContainerOffset.y;

        //PlaceOffset += ContainerOffset;
        
        ContainerRect.anchoredPosition = CurrentPosMouse + ContainerOffset;// + PlaceOffset;
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
        if (NeedsRefresh)
            return;

        if (TippedHovered && TextRect.localScale == Vector3.zero)
            TextRect.localScale = Vector3.one;
        //TextRect.localScale = Vector3.one;

        if (!TippedHovered && TextRect.localScale == Vector3.one)
            TextRect.localScale = Vector3.zero;
        //TextRect.localScale = Vector3.zero;
    }

    void Refresh()
    {
        if (!NeedsRefresh)
            return;

        Debug.Log("Refresh...");

        if (!RefreshPhaseDone[0])
        {
            TextRect.GetComponent<VerticalLayoutGroup>().enabled = false;
            TextRect.GetComponent<VerticalLayoutGroup>().enabled = true;
            RefreshPhaseDone[0] = true;
            return;
        }

        if (!RefreshPhaseDone[1])
        {
            Canvas.ForceUpdateCanvases();
            RefreshPhaseDone[1] = true;
            return;
        }

        if (!RefreshPhaseDone[2])
        {
            ContainerRect.GetComponent<HorizontalLayoutGroup>().enabled = false;
            ContainerRect.GetComponent<HorizontalLayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
            RefreshPhaseDone[2] = true;
            return;
        }

        ToolTipCanvas.sortingOrder = 4;

        for (int i = 0; i < RefreshPhaseDone.Length; i++)
            RefreshPhaseDone[i] = false;
        NeedsRefresh = false;
    }

    private void RefreshContentSize()
    {
        System.Collections.IEnumerator Routine()
        {
            var csf = ContainerRect.GetComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            yield return null;
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        this.StartCoroutine(Routine());
    }

    void RaycastUpdate(GraphicRaycaster myRaycaster)
    {
        if (myRaycaster == null)
            return;

        var pd = new PointerEventData(EventSystem.current);
        pd.position = Input.mousePosition;
        HitBuffer.Clear();
        myRaycaster.Raycast(pd, HitBuffer);
        
        //Debug.Log($"RaycastCount: {HitBuffer.Count}");
        foreach (RaycastResult result in HitBuffer)
        {
            //Debug.Log($"name: {result.gameObject.name}");

            if (result.gameObject == null)
                continue;

            if (result.gameObject.tag == GlobalConstants.TAG_BUTTON && !FoundRoot)
            {
                //Debug.Log("Found Button Tag!");

                RootUI candidate = result.gameObject.GetComponent<RootUI>();
                if (candidate != null)
                {
                    FoundRoot = true;

                    if (TargetRoot != candidate)
                    {
                        TargetRoot = candidate;
                        ToggleTip(TargetRoot);
                    }
                }   
            }

            if (result.gameObject.tag == GlobalConstants.TAG_PANEL && !FoundPage)
            {
                //Debug.Log("Found Panel Tag!");

                Page candidate = result.gameObject.GetComponent<Page>();
                if (candidate != null)
                {
                    //Debug.Log("Found Page!");

                    FoundPage = true;

                    if (TargetPage != candidate)
                    {
                        TargetPage = candidate;
                    }
                }
            }
        }

        
    }
    void RayCastSetup()
    {
        FoundRoot = false;
        FoundPage = false;
    }
    void RayCastCleanup()
    {
        if (!FoundRoot && TargetRoot != null)
        {
            TargetRoot = null;
            ToggleTip();
        }
        if (!FoundPage && TargetPage != null)
        {
            TargetPage = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        HitBuffer = new List<RaycastResult>();
        RefreshPhaseDone = new bool[3];
        PixelOffset = new Vector2(5, 5);
        //ToggleTip();


        //HitBuffer = new List<RaycastResult>();
    }

    // Update is called once per frame
    void Update()
    {
        CurrentPosMouse = (Vector2)Input.mousePosition;
        FollowMouse();
        if (JustFollow)
            return;

        RayCastSetup();
        RaycastUpdate(InGameRayCaster);
        RaycastUpdate(HUDrayCaster);
        RayCastCleanup();
        CheckClick();
        CheckText();
        Refresh();
    }
}
