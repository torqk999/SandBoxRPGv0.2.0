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

    public override bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        slotBin = character.EquipmentSlots;

        if (slotBin[(int)EquipSlot.OFF] != null &&
            !slotBin[(int)EquipSlot.OFF].UnEquipFromCharacter(character))
        {
            return false; // failed to remove the piece currently occupying the slot
        }

        if (!base.EquipToCharacter(character, slotBin, inventorySlot, (int)EquipSlot.OFF, subSlotIndex))
            return false;

        base.UpdateCharacterRender(character);
        return true;
    }
}
