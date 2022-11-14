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
        newEquip.InitializeSource();
        return newEquip;
    }
    public override void InitializeSource()
    {
        foreach (CharacterAbility ability in Abilities)
            ability.InitializeSource();
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

        for (int i = 0; i < Abilities.Length; i++)
        {
            if (source.Abilities[i] != null)
                Abilities[i] = source.Abilities[i].GenerateAbility(true);
            else
                Debug.Log($"Ability missing from id#{EquipID}:{Name}");
        }
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

        if (character.Abilities == null)
            return false;

        SlotFamily = character.EquipmentSlots;
        SlotIndex = slotIndex;
        SlotFamily[SlotIndex] = (Equipment)character.Inventory.RemoveIndexFromInventory(inventorySlot);

        foreach (CharacterAbility ability in Abilities)
            ability.EquipAbility(character, this);

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
            if (!(equipped is EffectAbility))
                continue;

            EffectAbility effectAbility = (EffectAbility)equipped;

            foreach (BaseEffect effect in effectAbility.Effects)
                foreach (BaseEffect spawnedEffect in effect.Logic.Clones)
                {
                    if (effect.Logic.Options.EffectType == EffectType.PASSIVE ||
                        effect.Logic.Options.EffectType == EffectType.TOGGLE)
                        Destroy(spawnedEffect);
                }
            
            effectAbility.Logic.SourceCharacter.Abilities.Remove(effectAbility);
            effectAbility.Logic.SourceCharacter.UpdateAbilites();
            effectAbility.Logic.SourceCharacter = null;
        }

        SlotFamily[SlotIndex] = null;
        SlotFamily = null;

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
