using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Hand
{
    //[Header("OneHand Properties")]
    //public int CurrentSlotIndex;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        Ring newRoot = (Ring)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }
    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        EquipSlot = EquipSlot.RING_0;
    }

    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is Ring))
            return;

        //Ring ringSource = (Ring)source;

        //CurrentSlotIndex = -1;

        
    }
    
    public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        slotIndex = (int)EquipSlot.RING_0;
        if (character.Slots.Equips[(int)EquipSlot.RING_0] != null &&
            character.Slots.Equips[(int)EquipSlot.RING_1] == null)
            slotIndex = (int)EquipSlot.RING_1;

        if (character.Slots.Equips[(int)EquipSlot.RING_0] != null &&
            character.Slots.Equips[(int)EquipSlot.RING_1] != null &&
            !((Equipment)character.Slots.Equips[(int)EquipSlot.RING_0]).UnEquipFromCharacter())
            return false;

        return base.EquipToCharacter(character, slotIndex);
    }
}
