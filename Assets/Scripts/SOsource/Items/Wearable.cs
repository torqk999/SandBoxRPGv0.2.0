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

    public override Equipment GenerateCloneEquip(int equipId = -1, bool inject = false, string instanceType = "wearable")
    {
        Wearable newWear = (Wearable)base.GenerateCloneEquip(equipId, inject, instanceType);
        newWear.Type = Type;
        return newWear;
    }
}