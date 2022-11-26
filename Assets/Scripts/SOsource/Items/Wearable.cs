using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wearable", menuName = "ScriptableObjects/Equipment/Wearable")]
public class Wearable : Equipment
{
    [Header("Wearable Properties")]
    //public EquipSlot WearSlot;
    public MaterialType BaseMaterial;
    public MaterialType TrimMaterial;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        Wearable newRoot = (Wearable)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }
    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is Wearable))
            return;

        Wearable wearSource = (Wearable)options.Source;
    }
    public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        slotIndex = (int)EquipSlot;
        if (character.Slots.Equips[slotIndex] != null &&
            !((Equipment)character.Slots.Equips[slotIndex]).UnEquipFromCharacter())
            return false;

        return base.EquipToCharacter(character, slotIndex);
    }
    public override bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (character == null ||
            character.Render == null)
            return false;



        return true;
    }
}