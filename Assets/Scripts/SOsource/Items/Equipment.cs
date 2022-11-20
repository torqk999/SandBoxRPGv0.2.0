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

    public WearSlot GetMySlot()
    {
        switch(this)
        {
            case OneHand:
                return WearSlot.MAIN;

            case Shield:
            case OffHand:
                return WearSlot.OFF;

            case Ring:
                return WearSlot.RING;

            case Wearable:
                return ((Wearable)this).WearSlot;
        }
        return default;
    }
    public override DraggableButton GenerateMyButton(ButtonOptions options)
    {
        options.ButtonType = ButtonType.DRAG;
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

    public virtual bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        UpdateCharacterRender(character);

        foreach (CharacterAbility ability in Abilities)
            ability.EquipAbility(character, this);

        return true;
    }
    public virtual bool UnEquipFromCharacter()
    {
        if (SlotPage == null ||
            SlotIndex < 0 ||
            SlotIndex >= SlotPage.Occupants.Length)
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
                foreach (BaseEffect spawnedEffect in effect.RootLogic.Clones)
                {
                    if (effect.Logic.Options.EffectType == EffectType.PASSIVE ||
                        effect.Logic.Options.EffectType == EffectType.TOGGLE)
                        Destroy(spawnedEffect);
                }
            
            effectAbility.Logic.SourceCharacter.Slots.Skill.Places.Remove(effectAbility.RootLogic.Button);
            effectAbility.Logic.SourceCharacter.UpdateAbilites();
            effectAbility.Logic.SourceCharacter = null;
        }

        SlotPage.Occupants[SlotIndex] = null;
        SlotPage = null;

        EquippedTo = null;
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
