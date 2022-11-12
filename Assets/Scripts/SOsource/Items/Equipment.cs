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

        EquipSchool = equipSource.EquipSchool;
        ClassType = equipSource.ClassType;
        EquipLevel = equipSource.EquipLevel;
        EquipID = equipId;

        CloneAbilities(equipSource);

        base.CloneItem(source);
    }
    void CloneAbilities(Equipment source)
    {
        Abilities = new CharacterAbility[source.Abilities.Length];
        Equipped = new CharacterAbility[Abilities.Length];
        for (int i = 0; i < Abilities.Length; i++)
        {
            Abilities[i] = source.Abilities[i].GenerateAbility(true);
        }
    }
    public virtual void Amplify(CharacterSheet sheet = null, Equipment equip = null)
    {
        foreach (CharacterAbility ability in Abilities)
            ability.Amplify(sheet, equip);
    }
    public virtual bool EquipToCharacter(Character character, ref int abilityId, int inventoryIndex = -1, int destinationIndex = -1)
    {
        return true;
        /*
        if (character == null ||
            character.Inventory == null)
            return false;

        if (inventoryIndex >= character.Inventory.Items.Count)
            return false;


        if (SlotFamily == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotFamily.Length)
            return false; // Nothing currently occupied, cannot push non-existant item into inventory

        if (inventoryIndex < 0 || destinationIndex == SlotIndex)
        {
            if (!character.Inventory.PushItemIntoInventory(this))
                return false;
            SlotFamily[SlotIndex] = null;
            SlotIndex = 0;
            UnEquipFromCharacter(character);
            return true; // Successfully removed
        }

        return false; // Switch between slots? 
        */
    }
    public virtual bool UnEquipFromCharacter(Character character)
    {
        if (SlotFamily == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotFamily.Length)
        {
            Debug.Log("Not equipped!");
            return false;
        }

        if (character.Inventory == null)
        {
            Debug.Log("Null inventory!");
            return false;
        }

        if (!character.Inventory.PushItemIntoInventory(this))
        {
            Debug.Log("No room in inventory!");
            return false;
        }    

        foreach (CharacterAbility equipped in Equipped)
            Destroy(equipped);

        UpdateCharacterRender(character, false);
        return true;
    }
    public void AppendAbilities(Character character, ref int abilityID)
    {
        for (int i = 0; i < Abilities.Length; i++)
        {
            Equipped[i] = Abilities[i].EquipAbility(character, abilityID, this);
            abilityID++;
        }
    }
    public virtual bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (character == null ||
            character.Render == null)
            return false;

        return true;
    }
}
