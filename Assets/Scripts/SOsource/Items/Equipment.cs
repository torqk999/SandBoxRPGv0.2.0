using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ItemObject
{
    [Header("Equipment Properties")]
    public School EquipSchool;
    public ClassType ClassType;

    public CharacterAbility[] Abilities;
    public CharacterAbility[] Equipped;

    [Header("Equip Logic - Do not touch")]
    public int EquipLevel;
    public int EquipID;

    public int SlotIndex;
    public Equipment[] SlotFamily;

    float GeneratePotency()
    {
        return 1 +

        (EquipLevel * CharacterMath.WEP_LEVEL_FACTOR);
    }
    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        Equipment newEquip = (Equipment)CreateInstance("Equipment");
        newEquip.CloneItem(this, equipId, inject);
        return newEquip;
    }
    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
        if (!(source is Equipment))
            return;

        Equipment equipSource = (Equipment)source;

        //EquipSkill = equipSource.EquipSkill;
        ClassType = equipSource.ClassType;

        EquipLevel = equipSource.EquipLevel;
        EquipID = equipId;

        CloneAbilities(equipSource);

        base.CloneItem(source);
    }
    void CloneAbilities(Equipment source)
    {
        Abilities = new CharacterAbility[source.Abilities.Length];
        for (int i = 0; i < Abilities.Length; i++)
        {
            Abilities[i] = source.Abilities[i].GenerateAbility(true);
        }
    }
    public virtual void Amplify(CharacterSheet sheet = null, Equipment equip = null)
    {
        foreach(CharacterAbility ability in Abilities)
            ability.Amplify(sheet, equip);
    }
    public virtual int EquipCharacter(Character character, int inventoryIndex = -1, int destinationIndex = -1)
    {
        if (character == null ||
            character.Inventory == null)
            return -1;

        if (inventoryIndex >= character.Inventory.Items.Count)
            return -1;

        if (SlotFamily == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotFamily.Length)
            return 0; // No currently found slot

        if (inventoryIndex < 0 || destinationIndex == SlotIndex) 
        {
            if (!character.Inventory.PushItemIntoInventory(this))
                return -1;
            SlotFamily[SlotIndex] = null;
            RemoveAbilitiesAndEffects(character);
            return 1; // Successfully removed
        }

        return 0; // Switch between slots? 
    }
    public void AppendAbilitiesAndEffects(Character character)
    {
        for (int i = 0; i < Abilities.Length; i++)
            Equipped[i] = Abilities[i].EquipAbility(character, this);
    }
    public void RemoveAbilitiesAndEffects(Character character)
    {

    }
    public virtual bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (character == null ||
            character.Render == null)
            return false;

        return true;
    }
}
