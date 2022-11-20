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
    [Header("Inventory Properties")]
    public Inventory MyInv;
    public override bool Drop()
    {
        return (UIMan.GameState.SceneMan.PushIntoContainer(UIMan.GameState.pController.CurrentCharacter, SlotIndex)) ;
    }
    public override bool Vacate()
    {
        if (!Root.Inventory.PushItemIntoInventory((ItemObject)Root))
            //!Drop()
            return false;
        return base.Vacate();
    }
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        if (!Following)
            UIMan.CharacterPageSelection(this);
        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        //SnapButton(Drop());
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        
        base.OnPointerEnter(eventData);
    }

    public override void Init(ButtonOptions options, RootScriptObject root = null)
    {
        base.Init(options, root);

        if (!(root is ItemObject))
            return;

        ItemObject item = (ItemObject)root;

        Stats.Append
           ($"GoldValue: {item.GoldValue}\n" +
            $"Quality: {item.Quality}\n" +
            $"Weight: {item.Weight}");
        ButtonText.text = (item is Stackable) ? ((Stackable)item).CurrentQuantity.ToString() : string.Empty;
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
