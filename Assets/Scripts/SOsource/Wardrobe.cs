using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wardrobe", menuName = "ScriptableObjects/Wardrobe")]
public class Wardrobe : ScriptableObject
{
    [Header("Equipment")]
    public Wearable Head;
    public Wearable Neck;
    public Wearable Pauldrons;
    public Wearable Chest;
    public Wearable Gloves;
    public Wearable Belt;
    public Wearable Legs;
    public Wearable Boots;

    [Header("Weapons")]
    public OneHand MyMainHand;
    public OffHand MyOffHand;
    public TwoHand MyTwoHand;

    [Header("Rings")]
    public Ring Slot1;
    public Ring Slot2;

    public int CloneAndEquipWardrobe(Character character, int equipId)
    {
        AttemptCloneAndEquipWear(character, Head, EquipSlot.HEAD, ref equipId);
        AttemptCloneAndEquipWear(character, Neck, EquipSlot.NECK, ref equipId);
        AttemptCloneAndEquipWear(character, Pauldrons, EquipSlot.PAULDRON, ref equipId);
        AttemptCloneAndEquipWear(character, Chest, EquipSlot.CHEST, ref equipId);
        AttemptCloneAndEquipWear(character, Gloves, EquipSlot.GLOVES, ref equipId);
        AttemptCloneAndEquipWear(character, Belt, EquipSlot.BELT, ref equipId);
        AttemptCloneAndEquipWear(character, Legs, EquipSlot.LEGS, ref equipId);
        AttemptCloneAndEquipWear(character, Boots, EquipSlot.BOOTS, ref equipId);

        AttemptCloneAndEquipRing(character, Slot1, 0, ref equipId);
        AttemptCloneAndEquipRing(character, Slot2, 1, ref equipId);

        if (!AttemptCloneAndEquipHand(character, MyTwoHand, ref equipId))
        {
            AttemptCloneAndEquipHand(character, MyMainHand, ref equipId);
            AttemptCloneAndEquipHand(character, MyOffHand, ref equipId);
        }

        character.UpdateAbilites();
        return equipId;
    }
    void AttemptCloneAndEquipRing(Character character, Ring ring, int slotIndex, ref int equipId)
    {
        if (ring == null || slotIndex < 0 || slotIndex >= character.RingSlots.Length)
            return;

        RingWrapper newRingWrap = new RingWrapper(ring, equipId, true);
        character.RingSlots[slotIndex] = newRingWrap;
        equipId++;
    }
    bool AttemptCloneAndEquipHand(Character character, Equipment hand, ref int equipId)
    {
        if (hand == null)
            return false;

        switch(hand)
        {
            case OneHand:
                OneHandWrapper newMain = new OneHandWrapper((OneHand)hand, equipId, true);
                character.EquipmentSlots[(int)EquipSlot.MAIN] = newMain;
                equipId++;
                return true;

            case OffHand:
                OffHandWrapper newOff = new OffHandWrapper((OffHand)hand, equipId, true);
                character.EquipmentSlots[(int)EquipSlot.OFF] = newOff;
                equipId++;
                return true;

            case TwoHand:
                TwoHandWrapper newTwo = new TwoHandWrapper((TwoHand)hand, equipId, true);
                character.EquipmentSlots[(int)EquipSlot.MAIN] = newTwo;
                character.EquipmentSlots[(int)EquipSlot.OFF] = newTwo;
                equipId++;
                return true;
        }
        return false;
    }
    void AttemptCloneAndEquipWear(Character character, Wearable wear, EquipSlot slot, ref int equipId)
    {
        WearableWrapper newWear = AttemptCloneWear(wear, slot, equipId);
        if (newWear != null)
        {
            character.EquipmentSlots[(int)slot] = newWear;
            equipId++;
        }
    }
    WearableWrapper AttemptCloneWear(Wearable wear, EquipSlot slot, int equipId)
    {
        if (wear != null && wear.Type == slot)
            return new WearableWrapper(wear, equipId, true);
        return null;
    }
}
