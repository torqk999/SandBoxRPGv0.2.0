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
        EquipSkill = SkillType.HAND_ONE;
        Type = oneSource.Type;
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

                if (slots[(int)EquipSlot.MAIN] != null &&
                    slots[(int)EquipSlot.MAIN].EquipCharacter(character, -1) == -1)
                {
                    return -1; // failed to remove the piece currently occupying the slot
                }

                SlotFamily = character.EquipmentSlots;
                SlotIndex = (int)EquipSlot.MAIN;
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