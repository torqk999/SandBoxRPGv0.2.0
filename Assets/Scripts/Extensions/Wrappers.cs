using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[System.Serializable]
public class ItemWrapper
{
    public int ItemID;
    public string Name;
    public Sprite Sprite; // <-- make sure this gets assigned at generation
    public GameObject Instantiation;
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
    public Equipment Equip;
}
[System.Serializable]
public class RingWrapper : EquipWrapper
{
    public int CurrentSlotIndex;

    public RingWrapper(Ring ring, int equipId = -1, bool inject = false)
    {
        Name = ring.Name;
        Sprite = ring.Sprite;
        Equip = ring.GenerateCloneEquip(equipId, inject);
        CurrentSlotIndex = -1;
    }

    public RingWrapper CloneWrapper()
    {
        return new RingWrapper((Ring)Equip);
    }
}
[System.Serializable]
public class WearableWrapper : EquipWrapper
{
    public WearableWrapper(Wearable wear, int equipId = -1, bool inject = false)
    {
        Name = wear.Name;
        Sprite = wear.Sprite;
        Equip = wear.GenerateCloneEquip(equipId, inject);
    }

    public WearableWrapper CloneWearWrapper()
    {
        return new WearableWrapper((Wearable)Equip);
    }
}
[System.Serializable]
public class OneHandWrapper : EquipWrapper
{
    public OneHandWrapper(OneHand oneHand, int equipId = -1, bool inject = false)
    {
        Name = oneHand.Name;
        Sprite = oneHand.Sprite;
        Equip = oneHand.GenerateCloneEquip(equipId, inject);
    }

    public OneHandWrapper CloneOneHandWrapper()
    {
        return new OneHandWrapper((OneHand)Equip);
    }
}
[System.Serializable]
public class OffHandWrapper : EquipWrapper
{
    public OffHandWrapper(OffHand offHand, int equipId = -1, bool inject = false)
    {
        Name = offHand.Name;
        Sprite = offHand.Sprite;
        Equip = offHand.GenerateCloneEquip(equipId, inject);
    }

    public OffHandWrapper CloneOffHandWrapper()
    {
        return new OffHandWrapper((OffHand)Equip);
    }
}
[System.Serializable]
public class TwoHandWrapper : EquipWrapper
{
    public TwoHandWrapper(TwoHand twoHand, int equipId = -1, bool inject = false)
    {
        Name = twoHand.Name;
        Sprite = twoHand.Sprite;
        Equip = twoHand.GenerateCloneEquip(equipId, inject);
    }

    public TwoHandWrapper CloneTwoHandWrapper()
    {
        return new TwoHandWrapper((TwoHand)Equip);
    }
}
public class ShieldWrapper : EquipWrapper
{
    public ShieldWrapper(Shield shield, int equipId = -1, bool inject = false)
    {
        Name = shield.Name;
        Sprite = shield.Sprite;
        Equip = shield.GenerateCloneEquip(equipId, inject);
    }

    public ShieldWrapper CloneShieldWrapper()
    {
        return new ShieldWrapper((Shield)Equip);
    }
}
*/