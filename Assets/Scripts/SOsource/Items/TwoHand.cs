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
        //EquipSkill = SkillType.HAND_TWO;
        Type = twoSource.Type;
    }
    public override bool EquipToCharacter(Character character, ref int abilityId, int inventorySlot, int destinationIndex = 0)
    {
        Equipment[] slots = character.EquipmentSlots;

        if (slots[(int)EquipSlot.OFF] != null &&
            !slots[(int)EquipSlot.OFF].UnEquipFromCharacter(character))
        { return false; }

        if (slots[(int)EquipSlot.MAIN] != null &&
            !slots[(int)EquipSlot.MAIN].UnEquipFromCharacter(character))
        { return false; }

        slots[(int)EquipSlot.MAIN] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
        slots[(int)EquipSlot.OFF] = slots[(int)EquipSlot.MAIN];

        SlotFamily = character.EquipmentSlots;
        SlotIndex = (int)EquipSlot.OFF;
        SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);
        AppendAbilities(character, ref abilityId);
        base.UpdateCharacterRender(character);
        return true;
    }
}