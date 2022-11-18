using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wearable", menuName = "ScriptableObjects/Equipment/Wearable")]
public class Wearable : Equipment
{
    [Header("Wearable Properties")]
    public EquipSlot EquipSlot;
    public MaterialType BaseMaterial;
    public MaterialType TrimMaterial;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "Wearable" : options.ClassID;
        Wearable newRoot = (Wearable)base.GenerateRootObject(options);
        newRoot.Clone(this, options);
        return newRoot;
    }
    public override void Clone(RootScriptObject source, RootOptions options)
    {
        base.Clone(source, options);

        if (!(source is Wearable))
            return;

        Wearable wearSource = (Wearable)source;
        EquipSlot = wearSource.EquipSlot;
    }
    public override bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        slotBin = character.EquipmentSlots;

        if (slotBin[(int)EquipSlot] != null && !slotBin[(int)EquipSlot].UnEquipFromCharacter(character))
        {
            return false;
        }

        if (!base.EquipToCharacter(character, slotBin, inventorySlot, (int)EquipSlot, subSlotIndex))
            return false;

        UpdateCharacterRender(character);
        return true;
    }
    public override bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (character == null ||
            character.Render == null)
            return false;



        return true;
    }
}