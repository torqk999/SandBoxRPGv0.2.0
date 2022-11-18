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
    public GameObject PanelTarget;
    public GraphicRaycaster MyRayCaster;
    public List<RaycastResult> HitBuffer;
    
    public void CheckOverLap(PointerEventData eventData)
    {
        HitBuffer.Clear();
        MyRayCaster.Raycast(eventData, HitBuffer);
        foreach(RaycastResult result in HitBuffer)
        {
            switch(result.gameObject.tag)
            {
                case GlobalConstants.TAG_BUTTON:

                    PlaceHolderButton place = result.gameObject.GetComponent<PlaceHolderButton>();

                    if (place == null)
                        continue;

                    ButtonTarget = place;
                    return;

                case GlobalConstants.TAG_PANEL:
                    break;
            }
        }
        ButtonTarget = null;
        return;
    }
    public override bool Vacate()
    {
        if (Place == null ||
            SlotFamily == null ||
            SlotIndex < 0)
            return false;

        Place = null;
        SlotFamily[SlotIndex] = null;
        SlotIndex = -1;
        return true;
    }
    public virtual bool Occupy(PlaceHolderButton place)
    {
        SlotFamily = place.SlotFamily;
        SlotIndex = place.SlotIndex;
        return true;
    }
    public void SnapButton(bool success = true)
    {
        if (success)
        {
            Place = ButtonTarget;
        }
        MyRect.position = Place.MyRect.position;
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
        CheckOverLap(eventData);
        if(ButtonTarget != null)
            SnapButton(ButtonTarget.Vacate() && Occupy(ButtonTarget));
    }
    protected override void Start()
    {
        base.Start();
        MyRayCaster = UIMan.GameMenuCanvas.GetComponent<GraphicRaycaster>();
        ButtonBounds.x = MyRect.rect.width / 2;
        ButtonBounds.y = MyRect.rect.height / 2;
    }
    public override void Update()
    {
        CurrentPosMouse = Input.mousePosition;
        FollowMouse(CurrentPosMouse);
    }
}
