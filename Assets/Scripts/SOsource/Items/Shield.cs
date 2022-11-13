using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShieldType
{
    BUCKLER,
    WOODEN,
    TOWER,
    KITE
}

[CreateAssetMenu(fileName = "Shield", menuName = "ScriptableObjects/Equipment/Shield")]
public class Shield : Hand
{
    [Header("Equipment Properties")]
    public ShieldType Type;

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        Shield newOneHand = (Shield)CreateInstance("Shield");
        newOneHand.CloneItem(this, equipId, inject);
        return newOneHand;
    }
    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is Shield))
            return;

        Shield shieldSource = (Shield)source;

        base.CloneItem(source, equipId, inject);

        Type = shieldSource.Type;
    }

    public override bool EquipToCharacter(Character character, ref int abilityId, int inventorySlot, int destinationIndex = 0)
    {
        Equipment[] slots = character.EquipmentSlots;

        if (slots[(int)EquipSlot.OFF] != null &&
            !slots[(int)EquipSlot.OFF].UnEquipFromCharacter(character))
        {
            return false; // failed to remove the piece currently occupying the slot
        }

        SlotFamily = character.EquipmentSlots;
        SlotIndex = (int)EquipSlot.OFF;
        SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
        //AppendAbilities(character, ref abilityId);
        base.UpdateCharacterRender(character);
        return true;
    }
}
