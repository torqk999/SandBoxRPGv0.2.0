using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemWrapper
{
    public int ItemID;
    public string Name;
    public Sprite Sprite; // <-- make sure this gets assigned at generation
}
[System.Serializable]
public class StackableWrapper : ItemWrapper
{
    public Stackable Item;
    public int CurrentQuantity;

    public StackableWrapper(Stackable item, int initialCount = 1)
    {
        Name = item.Name;
        Sprite = item.Sprite;
        Item = item.CloneStackable();
        CurrentQuantity = initialCount;
    }

    public StackableWrapper CloneWrapper()
    {
        return new StackableWrapper(Item);
    }
}
[System.Serializable]
public class EquipWrapper : ItemWrapper
{
    //public int AbilityID;
    public Equipment Equip;
    /*
    public EquipWrapper(Equipment equip)
    {
        Name = equip.Name;
        Sprite = equip.Sprite;
        //Equip = equip.CloneEquip();
    }

    
    public EquipWrapper CloneEquipWrapper()
    {
        return new EquipWrapper(Equip);
    }
    */
}
[System.Serializable]
public class RingWrapper : EquipWrapper
{
    public Ring Ring;
    public int CurrentSlotIndex;

    public RingWrapper(Ring ring)
    {
        Name = ring.Name;
        Sprite = ring.Sprite;
        Ring = ring.CloneRing();
        Equip = ring;
        CurrentSlotIndex = -1;
    }
}
[System.Serializable]
public class WearableWrapper : EquipWrapper
{
    public Wearable Wear;

    public WearableWrapper(Wearable wear)
    {
        Name = wear.Name;
        Sprite = wear.Sprite;
        Wear = wear.CloneWear();
        Equip = Wear;
    }

    public WearableWrapper CloneWearWrapper()
    {
        return new WearableWrapper(Wear);
    }
}
[System.Serializable]
public class OneHandWrapper : EquipWrapper
{
    public OneHand Hand;

    public OneHandWrapper(OneHand oneHand)
    {
        Name = oneHand.Name;
        Sprite = oneHand.Sprite;
        Hand = oneHand.CloneOneHand();
        Equip = Hand;
    }

    public OneHandWrapper CloneOneHandWrapper()
    {
        return new OneHandWrapper(Hand);
    }
}
[System.Serializable]
public class OffHandWrapper : EquipWrapper
{
    public OffHand Hand;

    public OffHandWrapper(OffHand offHand)
    {
        Name = offHand.Name;
        Sprite = offHand.Sprite;
        Hand = offHand.CloneOneHand();
        Equip = Hand;
    }

    public OffHandWrapper CloneOneHandWrapper()
    {
        return new OffHandWrapper(Hand);
    }
}
[System.Serializable]
public class TwoHandWrapper : EquipWrapper
{
    public TwoHand Hand;

    public TwoHandWrapper(TwoHand twoHand)
    {
        Name = twoHand.Name;
        Sprite = twoHand.Sprite;
        Hand = twoHand.CloneTwoHand();
        Equip = Hand;
    }

    public TwoHandWrapper CloneTwoHandWrapper()
    {
        return new TwoHandWrapper(Hand);
    }
}
