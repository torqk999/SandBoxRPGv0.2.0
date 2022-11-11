using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    public override Equipment GenerateCloneEquip(int equipId = -1, bool inject = false, string instanceType = "Ring")
    {
        Ring newRing = (Ring)base.GenerateCloneEquip(equipId, inject, instanceType);

        return newRing;
    }
}
