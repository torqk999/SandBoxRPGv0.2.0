using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(DraggableButton))]
public class DraggableButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        DraggableButton button = (DraggableButton)target;
    }
}

public class DraggableButton : SelectableButton
{
    [Header("DraggableButton")]
    public Vector2 CurrentPosMouse;
    public Vector2 OldPosButton;
    public Vector2 NewPosButton;
    public Vector2 Offset;
    //public Vector2 ButtonBounds;
    public Vector2 Buffer;
    public bool Following;

    [Header("Location Meta")]
    public PlaceHolderButton ButtonTarget;
    public RootScriptObject Root;
    public GraphicRaycaster MyRayCaster;
    public List<RaycastResult> HitBuffer;

    #region UPDATE
    bool CheckOverLap(PointerEventData eventData)
    {
        HitBuffer.Clear();
        MyRayCaster.Raycast(eventData, HitBuffer);
        ButtonTarget = null;
        bool OnMenu = false;
        foreach(RaycastResult result in HitBuffer)
        {
            Debug.Log($"result name: {result.gameObject.name}");

            switch(result.gameObject.tag)
            {
                case GlobalConstants.TAG_BUTTON:

                    PlaceHolderButton place = result.gameObject.GetComponent<PlaceHolderButton>();

                    Debug.Log($"place: {place != null}");
                    if (place != null)
                    {
                        Debug.Log($"index: {place.SlotIndex}");
                    }
                    if (place != null &&
                        place.CheckCanOccupy(this))
                    {
                        Debug.Log("PlaceHolderFound!");
                        ButtonTarget = place;
                    }
                        

                    break;

                case GlobalConstants.TAG_PANEL:
                    OnMenu = true;
                    break;
            }
        }
        return OnMenu;
    }
    void CheckSnapping()
    {
        if (SlotPage.Occupants.Places[SlotIndex] != null && !Following &&
            MyRect.anchoredPosition != SlotPage.Occupants.Places[SlotIndex].MyRect.anchoredPosition)
            MyRect.anchoredPosition = SlotPage.Occupants.Places[SlotIndex].MyRect.anchoredPosition;
            //SnapButton(Panel.Places[SlotIndex]);
    }
    void FollowMouse()
    {
        if (!Following)
            return;

        transform.position = CurrentPosMouse + Offset;
    }
    #endregion

    #region ACTION
    public virtual bool Drop()
    {
        return false;
    }
    public override bool Vacate()
    {
        if (SlotPage == null ||
            SlotPage.Occupants == null ||
            SlotPage.Occupants.Places == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotPage.Occupants.Places.Length )
            return false;

        SlotPage.Occupants.Places[SlotIndex] = null;
        SlotPage = null;
        SlotIndex = -1;

        // Un parent? Should be re-parenting anyway...
        return base.Vacate();
    }
    public bool Occupy(PlaceHolderButton place)
    {
        //return false;

        if (place == null)
            return false;

        if (!place.Vacate())
            return false;

        Debug.Log("pain...");

        //return false;

        Vacate();

        return false;
        SlotPage = place.SlotPage;
        SlotIndex = place.SlotIndex;
        SlotPage.Occupants[SlotIndex] = this;
        
        return true;
    }
    public void SnapButton(PlaceHolderButton button)
    {
        if (button == null)
            return;

        ButtonTarget = button;
        SnapButton();
    }
    public void SnapButton(bool panel = true, bool success = true)
    {
        Debug.Log($"panel: {panel} || success: {success}");
        return;

        if (!panel)
        {
            Drop();
        }
        if (success)
        {
            SlotPage = ButtonTarget.SlotPage;
        }
        /*Debug.Log($"Place Stuff:\n" +
            $"anchoredPositiion: {Place.MyRect.anchoredPosition}\n" +
            $"localPosition: {Place.MyRect.localPosition}\n" +
            $"position: {MyRect.position}");*/
        MyRect.SetParent(SlotPage.Occupants.PlaceContent);
        MyRect.anchoredPosition = SlotPage.Occupants.Places[SlotIndex].MyRect.anchoredPosition;
    }
    #endregion 

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        SetFollow(eventData);
        /*if (Following || UIMan.Dragging != null)
            return;

        Following = true;
        OldPosButton = MyRect.position;
        Offset = OldPosButton - CurrentPosMouse;
        UIMan.Dragging = this;*/
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Button release!");

        base.OnPointerUp(eventData);
       /* Following = false;
        SnapButton(CheckOverLap(eventData),
            ButtonTarget != null &&
            ButtonTarget.Vacate(this) &&
            Occupy(ButtonTarget));*/
    }
    void SetFollow(PointerEventData eventData)
    {
        Following = !Following;

        if (Following)
        {
            MyRect.SetParent(UIMan.HUDcanvas.transform);
            Offset = (Vector2)MyRect.position - CurrentPosMouse;
            UIMan.Dragging = this;
        }
        else
        {
            //transform.SetParent(Panel.OccupantContent);
            SnapButton(CheckOverLap(eventData), Occupy(ButtonTarget));
        }
    }
    public override void Init(ButtonOptions options, RootScriptObject root = null)
    {
        base.Init(options, root);
        if (UIMan == null)
        {
            Debug.Log("No UIMan found");
            return;
        }

        Root = root;

        if (Root != null)
        {
            if (Root.sprite != null)
            {
                //Debug.Log($"sprite name: {Root.sprite.name}");

                MyImage.sprite = Root.sprite;

                SpriteState ss = new SpriteState();

                ss.highlightedSprite = MyImage.sprite;
                ss.selectedSprite = MyImage.sprite;
                ss.pressedSprite = MyImage.sprite;
                ss.disabledSprite = MyImage.sprite;

                spriteState = ss;
            }


            Title.Append(Root.Name);
            Stats.Append("===Stats===\n");
            Flavour.Append(Root.Flavour);
        }

        Root.RootLogic.Button = this;

        

        transform.SetParent(SlotPage.Occupants.PlaceContent);
        transform.localScale = Vector3.one;

        //SlotFamily = options.Panel.Occupants;
        HitBuffer = new List<RaycastResult>();
        MyRayCaster = UIMan.GameMenuCanvas.GetComponent<GraphicRaycaster>();
    }
    protected override void Start()
    {
        base.Start();
        
    }
    
    public override void Update()
    {
        base.Update();
        CurrentPosMouse = Input.mousePosition;
        FollowMouse();
        CheckSnapping();
    }
}
