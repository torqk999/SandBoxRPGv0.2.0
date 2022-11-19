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
    public Vector2 ButtonBounds;
    public Vector2 Buffer;
    public bool Following;

    [Header("Location Meta")]
    public PlaceHolderButton Place;
    public PlaceHolderButton ButtonTarget;
    //public GameObject PanelTarget;
    public GraphicRaycaster MyRayCaster;
    public List<RaycastResult> HitBuffer;
    
    public bool CheckOverLap(PointerEventData eventData)
    {
        HitBuffer.Clear();
        MyRayCaster.Raycast(eventData, HitBuffer);
        bool OnMenu = false;
        foreach(RaycastResult result in HitBuffer)
        {
            switch(result.gameObject.tag)
            {
                case GlobalConstants.TAG_BUTTON:

                    PlaceHolderButton place = result.gameObject.GetComponent<PlaceHolderButton>();
                    
                    if (place != null &&
                        place.PlaceType == ReturnPlaceHolder(this))
                        ButtonTarget = place;

                    break;

                case GlobalConstants.TAG_PANEL:
                    OnMenu = true;
                    break;
            }
        }
        ButtonTarget = null;
        return OnMenu;
    }
    public virtual bool Drop()
    {
        return false;
    }
    public override bool Vacate(DraggableButton drag)
    {
        if (Place == null ||
            SlotFamily == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotFamily.Length )
            return false;

        Place = null;
        SlotFamily[SlotIndex] = null;
        SlotFamily = null;
        SlotIndex = -1;
        return base.Vacate(drag);
    }
    public virtual bool Occupy(PlaceHolderButton place)
    {
        SlotFamily = place.SlotFamily;
        SlotIndex = place.SlotIndex;
        SlotFamily[SlotIndex] = this;
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
            return;
        }
        if (success)
        {
            Place = ButtonTarget;
        }
        Debug.Log($"Place Stuff:\n" +
            $"anchoredPositiion: {Place.MyRect.anchoredPosition}\n" +
            $"localPosition: {Place.MyRect.localPosition}\n" +
            $"position: {MyRect.position}");

        MyRect.anchoredPosition = Place.MyRect.anchoredPosition;
    }
    void FollowMouse(Vector2 mousePos)
    {
        if (!Following)
            return;

        transform.position = CurrentPosMouse + Offset;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (Following || UIMan.Dragging != null)
            return;

        Following = true;
        OldPosButton = MyRect.position;
        Offset = OldPosButton - CurrentPosMouse;
        UIMan.Dragging = this;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        Following = false;
        SnapButton(CheckOverLap(eventData),
            ButtonTarget != null &&
            ButtonTarget.Vacate(this) &&
            Occupy(ButtonTarget));
    }
    void CheckSnapping()
    {
        if (Place != null && !Following &&
            MyRect.anchoredPosition != Place.MyRect.anchoredPosition)
            SnapButton(Place);
    }
    public override void Init()
    {
        base.Init();
        if (UIMan == null)
        {
            Debug.Log("No UIMan found");
            return;
        }

        HitBuffer = new List<RaycastResult>();
        MyRayCaster = UIMan.GameMenuCanvas.GetComponent<GraphicRaycaster>();
        ButtonBounds.x = MyRect.rect.width / 2;
        ButtonBounds.y = MyRect.rect.height / 2;
    }
    protected override void Start()
    {
        base.Start();
        
    }
    
    public override void Update()
    {
        base.Update();
        CurrentPosMouse = Input.mousePosition;
        FollowMouse(CurrentPosMouse);
        CheckSnapping();
    }
}
