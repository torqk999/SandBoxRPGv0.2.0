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
    public RootScriptObject Root;
    public bool Selected;
    public int SlotIndex;
    public PlaceHolderType PlaceType;
    public SelectableButton[] SlotFamily;

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
    public virtual bool Vacate(DraggableButton drag)
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
        PlaceType = options.PlaceType;
        SlotIndex = options.Index;

        if (SlotFamily != null)
            SlotFamily[SlotIndex] = this;

        transform.SetParent(options.Home);
        transform.localScale = Vector3.one;

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
