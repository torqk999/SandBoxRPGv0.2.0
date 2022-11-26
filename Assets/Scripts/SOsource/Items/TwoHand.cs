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

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        //options.Root = options.Root == "" ? "TwoHand" : options.Root;
        TwoHand newRoot = (TwoHand)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }
    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        EquipSlot = EquipSlot.MAIN;
    }
    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is TwoHand))
            return;

        TwoHand twoSource = (TwoHand)options.Source;
        Type = twoSource.Type;
    }
    /*public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        if (character.Slots.Equips.List[(int)EquipSlot.OFF] != null &&
            !((EquipmentButton)character.Slots.Equips.List[(int)EquipSlot.OFF]).UnEquipFromCharacter())
            return false;

        if (character.Slots.Equips.List[(int)EquipSlot.MAIN] != null &&
            !((EquipmentButton)character.Slots.Equips.List[(int)EquipSlot.MAIN]).UnEquipFromCharacter())
            return false;

        UpdateCharacterRender(character);
        return true;
    }*/
}