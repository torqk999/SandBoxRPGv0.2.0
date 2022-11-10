using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wearable", menuName = "ScriptableObjects/Equipment/Wearable")]
public class Wearable : Equipment
{
    [Header("Wearable Properties")]
    public EquipSlot Type;
    public MaterialType Base;
    public MaterialType Trim;
    //public EquipSkill EquipSkill;

    public Wearable CloneWear(int equipId = -1, bool inject = false)
    {
        Wearable newWear = (Wearable)CloneEquip("Wearable", equipId, inject);

        newWear.Type = Type;
        
        return newWear;
    }
}