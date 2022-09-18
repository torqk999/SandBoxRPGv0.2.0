using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    public Ring CloneRing(int equipId = -1, bool inject = false)
    {
        Ring newRing = (Ring)CloneEquip("Ring", equipId, inject);

        return newRing;
    }
}
