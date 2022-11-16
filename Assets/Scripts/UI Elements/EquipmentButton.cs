using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(EquipmentButton))]
public class EquipmentButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        EquipmentButton button = (EquipmentButton)target;
    }
}

public class EquipmentButton : InventoryButton
{
    [Header("Equipment")]
    public Equipment Equip;
    public Character EquippedTo;

    public override void Assign(ItemObject item)
    {
        Equip = (Equipment)item;
        base.Assign(item);
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        Stats.Append($"GearLevel: {Item.GoldValue}");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
