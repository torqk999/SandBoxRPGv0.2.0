using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OffHandType
{
    PARRY,
    TOTEM,
    RELIC,
    TORCH
}

[CreateAssetMenu(fileName = "OffHand", menuName = "ScriptableObjects/Equipment/OffHand")]
public class OffHand : Hand
{
    [Header("OffHand Properties")]
    public OffHandType Type;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = options.Root == "" ? "OffHand" : options.Root;
        OffHand newRoot = (OffHand)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        EquipSlot = EquipSlot.OFF;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is OffHand))
            return;

        OffHand offSource = (OffHand)source;
        Type = offSource.Type;
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
