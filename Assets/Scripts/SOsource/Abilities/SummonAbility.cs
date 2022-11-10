using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonAbility", menuName = "ScriptableObjects/Abilities/Summon")]
public class SummonAbility : CharacterAbility
{
    [Header("Summon Properties")]
    public Character SummonPrefab;
    public int Quantity;
    public float LifeSpan;

    public override void CloneAbility(CharacterAbility source, int equipId = -1, float potency = 1, bool inject = false)
    {
        base.CloneAbility(source, equipId);

        if (!(source is SummonAbility))
            return;

        SummonAbility summonSource = (SummonAbility)source;

        SummonPrefab = summonSource.SummonPrefab;
        Quantity = summonSource.Quantity;
        LifeSpan = summonSource.LifeSpan;
    }

    public override CharacterAbility EquipAbility(Character currentCharacter, Equipment equip, bool inject)
    {
        SummonAbility newSummonAbility = (SummonAbility)CreateInstance("SummonAbility");
        newSummonAbility.CloneAbility(this, equip.EquipID, GeneratePotency(currentCharacter, equip), inject);
        return newSummonAbility;
    }
}
