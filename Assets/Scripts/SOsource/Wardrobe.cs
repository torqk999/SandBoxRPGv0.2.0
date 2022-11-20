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
    /*
    public int CloneAndEquipWardrobe(Character character,ref int equipId)
    {
        AttemptCloneAndEquipWear(character, Head, WearSlot.HEAD, ref equipId);
        AttemptCloneAndEquipWear(character, Neck, WearSlot.NECK, ref equipId);
        AttemptCloneAndEquipWear(character, Pauldrons, WearSlot.PAULDRON, ref equipId);
        AttemptCloneAndEquipWear(character, Chest, WearSlot.CHEST, ref equipId);
        AttemptCloneAndEquipWear(character, Gloves, WearSlot.GLOVES, ref equipId);
        AttemptCloneAndEquipWear(character, Belt, WearSlot.BELT, ref equipId);
        AttemptCloneAndEquipWear(character, Legs, WearSlot.LEGS, ref equipId);
        AttemptCloneAndEquipWear(character, Boots, WearSlot.BOOTS, ref equipId);

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

        RootOptions options = new RootOptions(ref equipId);
        Ring newRing = (Ring)ring.GenerateRootObject(options);
        character.RingSlots[slotIndex] = newRing;
        //equipId++;
    }
    bool AttemptCloneAndEquipHand(Character character, Hand hand, ref int equipId)
    {
        if (hand == null)
            return false;

        RootOptions options = new RootOptions(ref equipId);
        switch (hand)
        {
            case OneHand:
                OneHand newMain = (OneHand)hand.GenerateRootObject(options);
                character.EquipmentSlots[(int)WearSlot.MAIN] = newMain;
                equipId++;
                return true;

            case OffHand:
                OffHand newOff = (OffHand)hand.GenerateRootObject(options);
                character.EquipmentSlots[(int)WearSlot.OFF] = newOff;
                equipId++;
                return true;

            case TwoHand:
                TwoHand newTwo = (TwoHand)hand.GenerateRootObject(options);
                character.EquipmentSlots[(int)WearSlot.MAIN] = newTwo;
                character.EquipmentSlots[(int)WearSlot.OFF] = newTwo;
                equipId++;
                return true;

            case Shield:
                Shield newShield = (Shield)hand.GenerateRootObject(options);
                character.EquipmentSlots[(int)WearSlot.OFF] = newShield;
                equipId++;
                return true;
        }
        return false;
    }
    void AttemptCloneAndEquipWear(Character character, Wearable wear, WearSlot slot, ref int equipId)
    {
        Wearable newWear = AttemptCloneWear(wear, slot, equipId);
        if (newWear != null)
        {
            character.EquipmentSlots[(int)slot] = newWear;
            equipId++;
        }
    }
    Wearable AttemptCloneWear(Wearable wearSource, WearSlot slot, int equipId)
    {
        if (wearSource != null && wearSource.WearSlot == slot)
        {
            RootOptions options = new RootOptions(ref equipId);
            return (Wearable)wearSource.GenerateRootObject(options);
        }   
        return null;
    }
    */
}
