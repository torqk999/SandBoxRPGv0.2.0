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
    public override int EquipCharacter(Character character, int inventorySlot, int destinationIndex = 0)
    {
        int callReturn = base.EquipCharacter(character, inventorySlot);

        switch (callReturn)
        {
            default: // failed action
                return -1;

            case 0:
                Equipment slot = character.EquipmentSlots[(int)EquipSlot];

                if (slot != null && slot.EquipCharacter(character, -1) == -1)
                {
                    return -1;
                }

                slot = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);

                SlotFamily = character.EquipmentSlots;
                SlotIndex = (int)EquipSlot.OFF;
                SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
                AppendAbilitiesAndEffects(character);
                UpdateCharacterRender(character);
                return 0;

            case 1:
                UpdateCharacterRender(character, false);
                return 1;
        }
    }
    public override bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (character == null ||
            character.Render == null)
            return false;



        return true;
    }
}