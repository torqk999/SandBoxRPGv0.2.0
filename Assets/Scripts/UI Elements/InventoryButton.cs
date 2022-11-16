using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(InventoryButton))]
public class InventoryButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        InventoryButton button = (InventoryButton)target;
    }
}

public class InventoryButton : DraggableButton
{
    [Header("Item")]
    public ItemObject Item;

    public virtual void Assign(ItemObject item)
    {
        Item = item;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        SnapButton(UIMan.ItemRelease(ref NewPosButton, this));
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        Stats.Clear();
        Stats.Append($"GoldValue: {Item.GoldValue}");
        base.OnPointerEnter(eventData);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Title.Append(Item.Name);
        Flavour.Append(Item.Flavour);
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
