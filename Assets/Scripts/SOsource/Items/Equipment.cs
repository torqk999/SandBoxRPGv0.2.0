using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ItemObject
{
    [Header("Equipment Properties")]
    public School EquipSchool;
    public ClassType ClassType;
    public CharacterAbility[] Abilities;

    [Header("Equip Logic - Do not touch")]
    //public CharacterAbility[] Equipped;
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
        //Equipped = new CharacterAbility[Abilities.Length];
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
    public virtual bool EquipToCharacter(Character character, Equipment[] slotBin = null, int inventorySlot = -1, int slotIndex = -1, int subSlotIndex = -1)
    {
        if (character == null ||
            slotBin == null ||
            slotIndex == -1)
            return false;

        if (character.Inventory == null ||
            inventorySlot < 0 ||
            inventorySlot >= character.Inventory.Items.Count)
            return false;

        SlotFamily = character.EquipmentSlots;
        SlotIndex = slotIndex;
        SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);

        return true;
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

        foreach (CharacterAbility equipped in Abilities)
        {
            foreach (BaseEffect effect in equipped.SpawnedEffects)
            {
                if (effect.EffectType == EffectType.PASSIVE ||
                    effect.EffectType == EffectType.TOGGLE)
                    Destroy(effect);
            }
            equipped.Source.Abilities.Remove(equipped);
            equipped.Source.UpdateAbilites();
            equipped.Source = null;
        }

        UpdateCharacterRender(character, false);
        return true;
    }
    public virtual bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (character == null ||
            character.Render == null)
            return false;

        return true;
    }
}
