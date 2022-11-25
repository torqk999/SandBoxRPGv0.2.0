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

class EquipmentButton : InventoryButton
{
    //[Header("Equipment")]
    //public Character EquippedTo;
    
    public bool EquipToCharacter(Character character)
    {
        return ((Equipment)Root).EquipToCharacter(character);
    }

    public override bool Vacate()
    {
        if (!base.Vacate())
            return false;

        UnEquipFromCharacter();
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
    public override void Init(ButtonOptions options)
    {
        base.Init(options);
        if (!(options.Root is Equipment))
            return;
        Equipment equip = (Equipment)options.Root;
        Strings[1].Append($"Equip Level: {equip.EquipLevel}");
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
