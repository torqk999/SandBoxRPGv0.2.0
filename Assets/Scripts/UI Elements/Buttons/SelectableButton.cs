using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(SelectableButton))]
public class SelectableButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        SelectableButton button = (SelectableButton)target;
    }
}

public class SelectableButton : TippedButton
{
    [Header("SelectableButton")]
    public Page Page;
    public int SlotIndex;
    public bool Selected;
    
    //public RootScriptObject Root;
    //public Transform SlotHome;
    //public SelectableButton[] SlotFamily;

    [Header("Debugging")]
    
    public Color DefaultColor;
    public Color HoverColor;
    public Color SelectionColor;

    /*public PlaceHolderType ReturnPlaceHolder(DraggableButton source)
    {
        switch (source)
        {
            case EquipmentButton:
                return PlaceHolderType.EQUIP;

            case InventoryButton:
                return PlaceHolderType.INVENTORY;

            case SkillButton:
                return PlaceHolderType.SKILL;
        }
        return default;
    }*/
    public virtual bool Vacate()
    {
        return true;
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        Selected = true;
        //MyImage.color = SelectionColor;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        //MyImage.color = HoverColor;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        //MyImage.color = DefaultColor;
    }
    public void UnSelect()
    {
        Selected = false;
        //MyImage.color = DefaultColor;
    }
    public override void Init(ButtonOptions options, RootScriptObject root = null)
    {
        base.Init(options, root);

        Page = options.Page;
        SlotIndex = options.Index;

        if (Page.Occupants.Places != null)
            Page.Occupants.Places[SlotIndex] = this;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
