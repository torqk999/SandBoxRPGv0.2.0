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

    public override CharacterAbility GenerateAbility(Character currentCharacter = null, bool inject = false, Equipment equip = null)
    {
        SummonAbility newAbility = (SummonAbility)CreateInstance("SummonAbility");
        int id = equip == null ? -1 : equip.EquipID;
        float potency = currentCharacter == null ? 1 : currentCharacter.GeneratePotency(equip);
        newAbility.CloneAbility(this, id, potency, inject);
        return newAbility;
    }
}
