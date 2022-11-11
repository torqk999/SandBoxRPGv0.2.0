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

        Ring newRing = (Ring)ring.GenerateItem(equipId, true);
        character.RingSlots[slotIndex] = newRing;
        equipId++;
    }
    bool AttemptCloneAndEquipHand(Character character, Hand hand, ref int equipId)
    {
        if (hand == null)
            return false;

        switch(hand)
        {
            case OneHand:
                OneHand newMain = (OneHand)hand.GenerateItem(equipId, true);
                character.EquipmentSlots[(int)EquipSlot.MAIN] = newMain;
                equipId++;
                return true;

            case OffHand:
                OffHand newOff = (OffHand)hand.GenerateItem(equipId, true);
                character.EquipmentSlots[(int)EquipSlot.OFF] = newOff;
                equipId++;
                return true;

            case TwoHand:
                TwoHand newTwo = (TwoHand)hand.GenerateItem(equipId, true);
                character.EquipmentSlots[(int)EquipSlot.MAIN] = newTwo;
                character.EquipmentSlots[(int)EquipSlot.OFF] = newTwo;
                equipId++;
                return true;

            case Shield:
                Shield newShield = (Shield)hand.GenerateItem(equipId, true);
                character.EquipmentSlots[(int)EquipSlot.OFF] = newShield;
                equipId++;
                return true;
        }
        return false;
    }
    void AttemptCloneAndEquipWear(Character character, Wearable wear, EquipSlot slot, ref int equipId)
    {
        Wearable newWear = AttemptCloneWear(wear, slot, equipId);
        if (newWear != null)
        {
            character.EquipmentSlots[(int)slot] = newWear;
            equipId++;
        }
    }
    Wearable AttemptCloneWear(Wearable wearSource, EquipSlot slot, int equipId)
    {
        if (wearSource != null && wearSource.Type == slot)
            return (Wearable)wearSource.GenerateItem(equipId, true);
        return null;
    }
}
