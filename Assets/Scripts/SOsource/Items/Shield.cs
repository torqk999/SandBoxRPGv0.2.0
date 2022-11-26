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

    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        EquipSlot = EquipSlot.OFF;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        Shield newRoot = (Shield)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }
    public override void Clone(RootOptions options)
    {
        if (!(options.Source is Shield))
            return;

        Shield shieldSource = (Shield)options.Source;

        base.Clone(options);

        Type = shieldSource.Type;
    }

    /*public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        slotIndex = (int)EquipSlot.OFF;
        if (character.Slots.Equips.List[slotIndex] != null &&
            !((EquipmentButton)character.Slots.Equips.List[slotIndex]).UnEquipFromCharacter())
            return false;

        return base.EquipToCharacter(character, slotIndex);
    }*/
}
