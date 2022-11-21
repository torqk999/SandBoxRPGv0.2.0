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
    public int EquipLevel;
    public Character EquippedTo;

    /*public WearSlot GetMySlot()
    {
        switch(this)
        {
            case OneHand:
                return HandSlot.MAIN;

            case Shield:
            case OffHand:
                return WearSlot.OFF;

            case Ring:
                return WearSlot.RING;

            case Wearable:
                return ((Wearable)this).WearSlot;
        }
        return default;
    }*/
    public override DraggableButton GenerateMyButton(ButtonOptions options)
    {
        options.ButtonType = ButtonType.ITEM;
        options.PlaceType = PlaceHolderType.EQUIP;
        GameObject buttonObject = RootLogic.GameState.UIman.GenerateButtonObject(options);
        EquipmentButton myButton = buttonObject.AddComponent<EquipmentButton>();
        myButton.Init(options, this);
        return myButton;
    }
    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == "" ? "Equipment" : options.ClassID;
        Equipment newRoot = (Equipment)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }
    public override void InitializeRoot(GameState state)
    {
        base.InitializeRoot(state);
        foreach (CharacterAbility ability in Abilities)
            ability.InitializeRoot(state);
    }
    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is Equipment))
            return;

        Equipment equipSource = (Equipment)source;

        EquipSchool = equipSource.EquipSchool;
        ClassType = equipSource.ClassType;
        EquipLevel = equipSource.EquipLevel;

        options.ID++; // MAYBE???
        CloneAbilities(equipSource, options);
    }
    void CloneAbilities(Equipment source, RootOptions options)
    {
        Abilities = new CharacterAbility[source.Abilities.Length];

        for (int i = 0; i < Abilities.Length; i++)
        {
            if (source.Abilities[i] != null)
            {
                Abilities[i] = source.Abilities[i].GenerateAbility(options);
            }
            else
                Debug.Log($"Ability missing from id#{RootLogic.Options.ID}:{Name}");
        }
    }

    public virtual int MyEquipIndex()
    {
        return -1;
    }

    public virtual bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        if (character == null)
            return false; // No character

        if (slotIndex == -1)
            return false; // Failed slot index

        if (character.Slots.Equips.Occupants.Places[slotIndex] == this)
            return false; // Already equipped here

        if (RootLogic.Button.Page.Occupants.Places == null ||
            RootLogic.Button.SlotIndex < 0 ||
            RootLogic.Button.SlotIndex >= character.Slots.Inventory.Occupants.Places.Count ||
            RootLogic.Button.Page.Occupants.Places[RootLogic.Button.SlotIndex] == null)
            return false; // Unable to source or missng

        if (character.Slots.Equips.Occupants.Places == null ||
            slotIndex < 0 ||
            slotIndex >= character.Slots.Equips.Occupants.Places.Count)
            return false; // Unable to find target

        List<SelectableButton> equipSlots = character.Slots.Equips.Occupants.Places;
        if (equipSlots[slotIndex] != null && !((Equipment)((EquipmentButton)equipSlots[slotIndex]).Root).UnEquipFromCharacter())
            return false; // failed to open up slot

        
        
        

        UpdateCharacterRender(character);

        foreach (CharacterAbility ability in Abilities)
            ability.EquipAbility(character, this);

        return true;
    }
    void RemapToSlot(Character character, int slotIndex, SlotPageType page)
    {
        RootLogic.Button.SlotIndex = slotIndex;
        switch (page)
        {
            case SlotPageType.INVENTORY:
                RootLogic.Button.Page = character.Slots.Inventory;
                break;

            case SlotPageType.EQUIPMENT:
                RootLogic.Button.Page = character.Slots.Equips;
                break;

            /*case SlotPageType.RINGS:
                RootLogic.Button.SlotPage = character.Slots.Rings;
                break;*/

            case SlotPageType.HOT_BAR:
                RootLogic.Button.Page = character.Slots.HotBar;
                break;

            case SlotPageType.SKILLS:
                RootLogic.Button.Page = character.Slots.Skills;
                break;
        }
        RootLogic.Button.Page.Occupants.Places[RootLogic.Button.SlotIndex] = RootLogic.Button;
    }

    public virtual bool UnEquipFromCharacter()
    {
        if (RootLogic.Button.Page == null ||
            RootLogic.Button.SlotIndex < 0 ||
            RootLogic.Button.SlotIndex >= RootLogic.Button.Page.Occupants.Places.Count)
        {
            Debug.Log("Not equipped!");
            return false;
        }

        if (EquippedTo.Slots.Inventory == null)
        {
            Debug.Log("Null inventory!");
            return false;
        }

        if (!EquippedTo.Slots.Inventory.PushItemIntoOccupants(this))
        {
            Debug.Log("No room in inventory!");
            return false;
        }
        RootLogic.Button.Page.Occupants.Places[RootLogic.Button.SlotIndex] = null;
        RootLogic.Button.Page = null;

        foreach (CharacterAbility equipped in Abilities)
        {
            if (!(equipped is EffectAbility))
                continue;

            EffectAbility effectAbility = (EffectAbility)equipped;

            foreach (BaseEffect effect in effectAbility.Effects)
                foreach (BaseEffect spawnedEffect in effect.RootLogic.Clones)
                {
                    if (effect.Logic.Options.EffectType == EffectType.PASSIVE ||
                        effect.Logic.Options.EffectType == EffectType.TOGGLE)
                        Destroy(spawnedEffect);
                }
            
            effectAbility.Logic.SourceCharacter.Slots.Skills.Occupants.Places.Remove(effectAbility.RootLogic.Button);
            effectAbility.Logic.SourceCharacter.UpdateAbilites();
            effectAbility.Logic.SourceCharacter = null;
        }

        EquippedTo = null;
        UpdateCharacterRender(EquippedTo, false);
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
