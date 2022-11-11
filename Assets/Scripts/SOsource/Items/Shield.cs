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
        //EquipSkill = SkillType.SHIELD;
        Type = shieldSource.Type;
    }

    public override int EquipCharacter(Character character, int inventorySlot, int destinationIndex = 0)
    {
        int callReturn = base.EquipCharacter(character, inventorySlot);

        switch(callReturn)
        {
            default: // failed action
                return -1;

            case 0:
                Equipment[] slots = character.EquipmentSlots;

                if (slots[(int)EquipSlot.OFF] != null &&
                    slots[(int)EquipSlot.OFF].EquipCharacter(character, -1) == -1)
                {
                    return -1; // failed to remove the piece currently occupying the slot
                }

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
