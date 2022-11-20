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
    //[Header("Equipment")]
    //public Character EquippedTo;
    
    public bool EquipToCharacter(Character character)
    {
        if (character == null)// ||
            //character.Slots == null ||
            //character.Slots.Equips == null)
            return false; // Missing Essentials

        int slotIndex = 

        if (character.Slots.Equips.Occupants.Places[slotIndex] == this)
            return false; // Already equipped here

        if (SlotPage.Occupants.Places == null ||
            SlotIndex < 0 ||
            SlotIndex >= character.Slots.Inventory.Occupants.Places.Length ||
            SlotPage.Occupants.Places[SlotIndex] == null)
            return false; // Unable to source or missng

        if (character.Slots.Equips.Occupants.Places == null ||
            slotIndex < 0 ||
            slotIndex >= character.Slots.Equips.Occupants.Places.Length)
            return false; // Unable to find target

        EquipmentButton[] equipSlots = (EquipmentButton[])character.Slots.Equips.Occupants.Places;
        if (equipSlots[slotIndex] != null && !((Equipment)equipSlots[slotIndex].Root).UnEquipFromCharacter())
            return false; // failed to open up slot

        if (!((Equipment)Root).EquipToCharacter(character))
            return false; // failed to equip character

        SlotIndex = slotIndex;
        SlotPage = character.Slots.Equips;
        SlotPage.Occupants.Places[SlotIndex] = this;


        

        return true;
    }

    public bool UnEquipFromCharacter()
    {
        return ((Equipment)Root).UnEquipFromCharacter();
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
    }
    public override void Init(ButtonOptions options, RootScriptObject root = null)
    {
        base.Init(options, root);
        if (!(root is Equipment))
            return;
        Equipment equip = (Equipment)root;
        Stats.Append($"Equip Level: {equip.EquipLevel}");
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
