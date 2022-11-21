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
        options.ClassID = options.ClassID == "" ? "TwoHand" : options.ClassID;
        TwoHand newRoot = (TwoHand)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is TwoHand))
            return;

        TwoHand twoSource = (TwoHand)source;
        Type = twoSource.Type;
    }
    public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        if (character.Slots.Equips.List[(int)EquipSlot.MAIN] != null &&
            !((EquipmentButton)character.Slots.Equips.List[(int)EquipSlot.MAIN]).UnEquipFromCharacter())
            return false;

        if (character.Slots.Equips.List[(int)EquipSlot.OFF] != null &&
            !((EquipmentButton)character.Slots.Equips.List[(int)EquipSlot.OFF]).UnEquipFromCharacter())
            return false;

        UpdateCharacterRender(character);
        return true;
    }
}