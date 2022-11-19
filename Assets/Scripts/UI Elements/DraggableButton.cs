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
    public PlaceHolderButton PlaceHolder;
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
                        place.PlaceType == PlaceType)
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
        if (PlaceHolder != null && !Following &&
            MyRect.anchoredPosition != PlaceHolder.MyRect.anchoredPosition)
            SnapButton(PlaceHolder);
    }
    void FollowMouse(Vector2 mousePos)
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
    public override bool Vacate(DraggableButton drag)
    {
        if (PlaceHolder == null ||
            SlotFamily == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotFamily.Length )
            return false;

        PlaceHolder = null;
        SlotFamily[SlotIndex] = null;
        SlotFamily = null;
        SlotIndex = -1;
        return base.Vacate(drag);
    }
    public virtual bool Occupy(PlaceHolderButton place)
    {
        //if ()
        SlotFamily = place.OccupantSlots;
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
            PlaceHolder = ButtonTarget;
        }
        /*Debug.Log($"Place Stuff:\n" +
            $"anchoredPositiion: {Place.MyRect.anchoredPosition}\n" +
            $"localPosition: {Place.MyRect.localPosition}\n" +
            $"position: {MyRect.position}");*/

        MyRect.anchoredPosition = PlaceHolder.MyRect.anchoredPosition;
    }
    #endregion 

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
        Debug.Log("Button release!");

        base.OnPointerUp(eventData);
        Following = false;
        SnapButton(CheckOverLap(eventData),
            ButtonTarget != null &&
            ButtonTarget.Vacate(this) &&
            Occupy(ButtonTarget));
    }
    
    public override void Init(ButtonOptions options, RootScriptObject root = null)
    {
        base.Init(options, root);
        if (UIMan == null)
        {
            Debug.Log("No UIMan found");
            return;
        }

        PlaceHolder = options.PlaceHolder;
        SlotFamily = (SelectableButton[])options.OccupantFolder;
        HitBuffer = new List<RaycastResult>();
        MyRayCaster = UIMan.GameMenuCanvas.GetComponent<GraphicRaycaster>();
        //ButtonBounds.x = MyRect.rect.width / 2;
        //ButtonBounds.y = MyRect.rect.height / 2;
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
