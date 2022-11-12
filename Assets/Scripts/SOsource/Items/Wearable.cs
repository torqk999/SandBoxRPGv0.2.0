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

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        Wearable newOneHand = (Wearable)CreateInstance("Wearable");
        newOneHand.CloneItem(this, equipId, inject);
        return newOneHand;
    }
    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is Wearable))
            return;

        Wearable wearSource = (Wearable)source;

        base.CloneItem(source, equipId, inject);
        //EquipSkill = SkillType.MEDIUM;
        EquipSlot = wearSource.EquipSlot;
    }
    public override bool EquipToCharacter(Character character, ref int abilityId, int inventorySlot, int destinationIndex = 0)
    {

        Equipment slot = character.EquipmentSlots[(int)EquipSlot];

        if (slot != null && !slot.UnEquipFromCharacter(character))
        {
            return false;
        }

        slot = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);

        SlotFamily = character.EquipmentSlots;
        SlotIndex = (int)EquipSlot.OFF;
        SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
        AppendAbilities(character, ref abilityId);
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