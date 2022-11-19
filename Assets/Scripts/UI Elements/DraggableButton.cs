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
    //public PlaceHolderButton PlaceHolder;
    public PlaceHolderButton ButtonTarget;
    //public GameObject PanelTarget;
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
        if (Panel.Places[SlotIndex] != null && !Following &&
            MyRect.anchoredPosition != Panel.Places[SlotIndex].MyRect.anchoredPosition)
            SnapButton(Panel.Places[SlotIndex]);
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
        if (Panel == null ||
            Panel.Occupants == null ||
            Panel.OccupantContent == null ||
            SlotIndex < 0 ||
            SlotIndex >= Panel.Occupants.Length )
            return false;

        Panel.Occupants[SlotIndex] = null;
        Panel = null;
        SlotIndex = -1;

        // Un parent? Should be re-parenting anyway...
        return base.Vacate();
    }
    public bool Occupy(PlaceHolderButton place)
    {
        if (place == null)
            return false;

        if (!place.Vacate())
            return false;

        Vacate();
        Panel = place.Panel;
        SlotIndex = place.SlotIndex;
        Panel.Occupants[SlotIndex] = this;
        
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
        if (!panel)
        {
            Drop();
        }
        if (success)
        {
            Panel = ButtonTarget.Panel;
        }
        /*Debug.Log($"Place Stuff:\n" +
            $"anchoredPositiion: {Place.MyRect.anchoredPosition}\n" +
            $"localPosition: {Place.MyRect.localPosition}\n" +
            $"position: {MyRect.position}");*/
        MyRect.SetParent(Panel.OccupantContent);
        MyRect.anchoredPosition = Panel.Places[SlotIndex].MyRect.anchoredPosition;
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
            transform.SetParent(UIMan.HUDcanvas.transform);
            Offset = (Vector2)MyRect.position - CurrentPosMouse;
            UIMan.Dragging = this;
        }
        else
        {
            transform.SetParent(Panel.OccupantContent);
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

        if(Root != null)
            Root.RootLogic.Button = this;

        if (Panel.Occupants != null)
            Panel.Occupants[SlotIndex] = this;

        transform.SetParent(Panel.OccupantContent);
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
