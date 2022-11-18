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

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "Shield" : options.ClassID;
        Shield newRoot = (Shield)base.GenerateRootObject(options);
        newRoot.Clone(this, options);
        return newRoot;
    }
    public override void Clone(RootScriptObject source, RootOptions options)
    {
        if (!(source is Shield))
            return;

        Shield shieldSource = (Shield)source;

        base.Clone(source, options);

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
