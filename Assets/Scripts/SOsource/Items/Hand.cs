using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Equipment
{
    [Header("Hand Properties")]
    public MaterialType Base;
    public MaterialType Handle;
    public List<MaterialType> Details;

    public override Equipment GenerateCloneEquip(int equipId = -1, bool inject = false, string instanceType = "Hand")
    {
        Hand hand = (Hand)base.GenerateCloneEquip(equipId, inject, instanceType);

        hand.Base = Base;
        hand.Handle = Handle;
        hand.Details = new List<MaterialType>();
        hand.Details.AddRange(Details);

        return hand;
    }
}
