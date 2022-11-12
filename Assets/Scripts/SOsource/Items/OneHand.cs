using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OneHandType
{
    DAGGER,
    WAND,
    RAPIER,
    AXE,
    MACE,
    FLAIL
}

[CreateAssetMenu(fileName = "OneHand", menuName = "ScriptableObjects/Equipment/OneHand")]
public class OneHand : Hand
{
    [Header("OneHand Properties")]
    public OneHandType Type;

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        OneHand newOneHand = (OneHand)CreateInstance("OneHand");
        newOneHand.CloneItem(this, equipId, inject);
        return newOneHand;
    }
    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is OneHand))
            return;

        OneHand oneSource = (OneHand)source;

        base.CloneItem(source, equipId, inject);

        Type = oneSource.Type;
    }
    public override bool EquipToCharacter(Character character, ref int abilityId, int inventorySlot, int destinationIndex = 0)
    {
        Equipment[] slots = character.EquipmentSlots;

        if (slots[(int)EquipSlot.MAIN] != null &&
            !slots[(int)EquipSlot.MAIN].UnEquipFromCharacter(character))
        {
            return false; // failed to remove the piece currently occupying the slot
        }

        SlotFamily = character.EquipmentSlots;
        SlotIndex = (int)EquipSlot.MAIN;
        SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
        AppendAbilities(character, ref abilityId);
        base.UpdateCharacterRender(character);
        return true;
    }
}