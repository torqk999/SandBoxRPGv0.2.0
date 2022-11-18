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
    public override ExtendedButton GenerateButton(GameObject prefab, Transform folder)
    {
        GameObject buttonObject = GenerateButtonObject(prefab, folder);
        InventoryButton newButton = buttonObject.AddComponent<InventoryButton>();
        return newButton;
    }
    public override void Assign(RootScriptObject root)
    {
        base.Assign(root);
        if (!(root is ItemObject))
            return;

        ItemObject item = (ItemObject)root;

        Stats.Append($"GoldValue: {item.GoldValue}\n" +
            $"Quality: {item.Quality}\n" +
            $"Weight: {item.Weight}");
        ButtonText.text = (item is Stackable) ? ((Stackable)item).CurrentQuantity.ToString() : string.Empty;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        UIMan.CharacterPageSelection(this);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        SnapButton(UIMan.ItemRelease(ref NewPosButton, this));
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        
        base.OnPointerEnter(eventData);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MyImage.sprite = Root.Sprite;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
