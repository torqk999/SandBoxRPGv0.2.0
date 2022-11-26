using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : ItemObject
{
    [Header("Equipment Properties")]
    public School EquipSchool;
    public ClassType ClassType;
    public List<CharacterAbility> Abilities;

    [Header("Equip Logic - Do not touch")]
    public int EquipLevel;
    public EquipSlot EquipSlot;
    public Character EquippedTo;

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        Equipment newRoot = (Equipment)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }
    public override void InitializeRoot(GameState state)
    {
        Debug.Log("Initializing Equip...");
        base.InitializeRoot(state);
        foreach (CharacterAbility ability in Abilities)
            ability.InitializeRoot(state);
    }
    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is Equipment))
            return;

        Equipment equipSource = (Equipment)options.Source;

        EquipSchool = equipSource.EquipSchool;
        ClassType = equipSource.ClassType;
        EquipLevel = equipSource.EquipLevel;
        EquipSlot = equipSource.EquipSlot;

        CloneAbilities(equipSource, options);

        Debug.Log("Copy Equipment Complete!");
    }
    void CloneAbilities(Equipment source, RootOptions options)
    {
        Debug.Log("Cloning equip abilities...");

        Abilities = new List<CharacterAbility>();

        for (int i = 0; i < (source.Abilities.Count); i++)
        {
            if (source.Abilities[i] != null)
            {
                options.Source = source.Abilities[i];
                options.Index = i;
                options.ID++; // MAYBE???
                Abilities.Add((CharacterAbility)source.Abilities[i].GenerateRootObject(options));
                Debug.Log("Ability added to new equip!");
            }
            else
                Debug.Log($"Ability missing from id#{RootLogic.Options.ID}:{Name}");
        }

        Debug.Log("Equip abilities generated!");
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
        if (EquippedTo == null)
            return true;
        /////// DONT LOSE THIS ////////////////

        foreach (CharacterAbility equipped in Abilities)
        {
            if (equipped is EffectAbility)
            {
                EffectAbility effectAbility = (EffectAbility)equipped;

                foreach (BaseEffect effect in effectAbility.Effects)
                    foreach (BaseEffect spawnedEffect in effect.RootLogic.Clones)
                    {
                        if (effect.Logic.Options.EffectType == EffectType.PASSIVE ||
                            effect.Logic.Options.EffectType == EffectType.TOGGLE)
                            Destroy(spawnedEffect);
                    }

                RootLogic.Options.GameState.UIman.Equipments.Remove(effectAbility);
                //effectAbility.Logic.SourceCharacter.UpdateAbilites();
                effectAbility.Logic.SourceCharacter = null;
            }
            equipped.Vacate();
        }

        //////////////////////////////////////

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
