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
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        if (!(source is Shield))
            return;

        Shield shieldSource = (Shield)source;

        base.Copy(source, options);

        Type = shieldSource.Type;
    }

    public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        slotIndex = (int)HandSlot.OFF;
        if (character.Slots.Equips.Occupants.Places[slotIndex] != null &&
            !((EquipmentButton)character.Slots.Equips.Occupants.Places[slotIndex]).UnEquipFromCharacter())
            return false;

        return base.EquipToCharacter(character, slotIndex);
    }
}
