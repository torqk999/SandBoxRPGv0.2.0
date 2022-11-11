using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TwoHandType
{
    AXE,
    CLAYMORE,
    POLEARM,
    BARDICHE,
    SPEAR,
    STAFF,
    BOW,
    CROSSBOW
}

[CreateAssetMenu(fileName = "TwoHand", menuName = "ScriptableObjects/Equipment/TwoHand")]
public class TwoHand : Hand
{
    [Header("TwoHand Properties")]
    public TwoHandType Type;

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        TwoHand newOneHand = (TwoHand)CreateInstance("TwoHand");
        newOneHand.CloneItem(this, equipId, inject);
        return newOneHand;
    }

    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is TwoHand))
            return;

        TwoHand twoSource = (TwoHand)source;

        base.CloneItem(source, equipId, inject);

        Type = twoSource.Type;
    }

    public override int EquipCharacter(Character character, int inventorySlot, int destinationIndex = 0)
    {
        int callReturn = base.EquipCharacter(character, inventorySlot);

        switch (callReturn)
        {
            default: // failed action
                return -1;

            case 0:
                Equipment[] slots = character.EquipmentSlots;

                if (slots[(int)EquipSlot.OFF] != null &&
                    slots[(int)EquipSlot.OFF].EquipCharacter(character, -1) == -1)
                { return -1; }

                if (slots[(int)EquipSlot.MAIN] != null &&
                    slots[(int)EquipSlot.MAIN].EquipCharacter(character, -1) == -1)
                { return -1; }

                slots[(int)EquipSlot.MAIN] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
                slots[(int)EquipSlot.OFF] = slots[(int)EquipSlot.MAIN];

                SlotFamily = character.EquipmentSlots;
                SlotIndex = (int)EquipSlot.OFF;
                SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
                AppendAbilitiesAndEffects(character);
                base.UpdateCharacterRender(character);
                return 0;

            case 1:
                base.UpdateCharacterRender(character, false);
                return 1;
        }
    }
}