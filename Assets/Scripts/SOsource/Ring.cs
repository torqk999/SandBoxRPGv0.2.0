using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ring", menuName = "ScriptableObjects/Equipment/Ring")]
public class Ring : Equipment
{
    public Ring CloneRing(bool inject = false)
    {
        Ring newRing = (Ring)CloneEquip("Ring", inject);

        return newRing;
    }
}
