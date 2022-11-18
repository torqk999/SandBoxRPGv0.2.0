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
    public List<DraggableButton> SlotFamily;

    [Header("Debugging")]
    
    public Color DefaultColor;
    public Color HoverColor;
    public Color SelectionColor;

    public virtual bool Vacate()
    {
        return true;
    }
    public virtual void Assign(RootScriptObject root)
    {
        Root = root;

        if (Root == null)
            return;

        if (Root.Sprite != null)
            MyImage.sprite = Root.Sprite;

        Title.Clear();
        Title.Append(Root.Name);
        Stats.Clear();
        Stats.Append("===Stats===\n");
        Flavour.Clear();
        Flavour.Append(Root.Flavour);

        //ButtonText.text = (root is Stackable) ? ((Stackable)root).CurrentQuantity.ToString() : string.Empty;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //UIMan.CharacterPageSelection(this);
        //UIMan.StrategyPageSelection(this);
        Selected = true;
        MyImage.color = SelectionColor;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        MyImage.color = HoverColor;
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        MyImage.color = DefaultColor;
    }
    public void UnSelect()
    {
        Selected = false;
        MyImage.color = DefaultColor;
    }

    

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DefaultColor = MyImage.color;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
