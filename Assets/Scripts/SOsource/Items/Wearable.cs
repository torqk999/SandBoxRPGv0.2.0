using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wearable", menuName = "ScriptableObjects/Equipment/Wearable")]
public class Wearable : Equipment
{
    [Header("Wearable Properties")]
    public EquipSlot WearSlot;
    public MaterialType BaseMaterial;
    public MaterialType TrimMaterial;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "Wearable" : options.ClassID;
        Wearable newRoot = (Wearable)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is Wearable))
            return;

        Wearable wearSource = (Wearable)source;
        WearSlot = wearSource.WearSlot;
    }
    public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        slotIndex = (int)WearSlot;
        if (character.Slots.Equips.List[slotIndex] != null &&
            !((EquipmentButton)character.Slots.Equips.List[slotIndex]).UnEquipFromCharacter())
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