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
    //public Panel Panel;
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
        foreach (RaycastResult result in HitBuffer)
        {
            Debug.Log($"result name: {result.gameObject.name}");

            switch (result.gameObject.tag)
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
        if (Panel.List[SlotIndex] != null && !Following &&
            MyRect.anchoredPosition != Panel.List[SlotIndex].MyRect.anchoredPosition)
            MyRect.anchoredPosition = Panel.List[SlotIndex].MyRect.anchoredPosition;
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
        return Vacate();
    }
    public virtual bool Vacate()
    {
        if (Panel == null ||
            Panel.List == null ||
            SlotIndex < 0 ||
            SlotIndex >= Panel.List.Count)
            return false;

        Panel.List[SlotIndex] = null;
        return true;
        // Un parent? Should be re-parenting anyway...
        //return base.Vacate();
    }
    public bool Relocate()
    {
        if (Panel == null ||
            Panel.List == null)
            return false;

        int emptyIndex = Panel.ReturnEmptyIndex();
        if (emptyIndex == -1)
        {
            return Drop();
        }

        Panel.List[SlotIndex] = null;
        SlotIndex = emptyIndex;
        Panel.List[SlotIndex] = this;
        return true;
    }
    public bool CheckCanOccupy(PlaceHolderButton place)
    {
        if (place == null)
            return false;

        if (place.Panel.VirtualParent.Occupants.List[ButtonTarget.SlotIndex] == this)
            return false; // already there;

        if (!place.Vacate())
            return false;

        return true;
    }
    public virtual bool Occupy(PlaceHolderButton place)
    {
        if (!CheckCanOccupy(place))
            return false;

        Panel = place.Panel.VirtualParent.Occupants;
        SlotIndex = place.SlotIndex;
        Panel.List[SlotIndex] = this;

        return true;
    }

    public void SnapButton()
    {
        MyRect.SetParent(Panel.PhysicalParent);
        Debug.Log($"SlotIndex: {SlotIndex}\n");
        Debug.Log($"Panel: {Panel != null}\n");
        Debug.Log($"VirtualParent: {Panel.VirtualParent != null}\n");
        Debug.Log($"PlaceHolders: {Panel.VirtualParent.PlaceHolders != null}\n");
        Debug.Log($"Button: {Panel.VirtualParent.PlaceHolders.List[SlotIndex] != null}");
        MyRect.anchoredPosition = Panel.VirtualParent.PlaceHolders.List[SlotIndex].MyRect.anchoredPosition;
    }
    #endregion 

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        SetFollow(eventData);
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
            return;
        }

        if (!CheckOverLap(eventData))
        {
            Drop();
            return;
        }

        if(Occupy(ButtonTarget))
            Vacate();
            
        SnapButton();
    }
    public override void Init(ButtonOptions options)
    {
        base.Init(options);
        if (UIMan == null)
        {
            Debug.Log("No UIMan found");
            return;
        }

        Root = options.Root;

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

        transform.SetParent(Panel.PhysicalParent);
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
