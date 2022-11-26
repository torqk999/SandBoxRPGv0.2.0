using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Equipment
{
    [Header("Hand Properties")]
    //public HandSlot HandSlot;
    public HandPosition HandPosition;
    public MaterialType BaseMaterial;
    public MaterialType HandleMaterial;
    public MaterialType TrimMaterial;
    //public List<MaterialType> Details;
    public GameObject Instantiation;   // Used by equipping only

    public override RootScriptObject GenerateRootObject(RootOptions options)
    {
        Hand newRoot = (Hand)base.GenerateRootObject(options);
        newRoot.Clone(options);
        return newRoot;
    }

    public override void Clone(RootOptions options)
    {
        base.Clone(options);

        if (!(options.Source is Hand))
            return;

        Hand handSource = (Hand)options.Source;

        HandPosition = handSource.HandPosition;
        BaseMaterial = handSource.BaseMaterial;
        HandleMaterial = handSource.HandleMaterial;
        TrimMaterial = handSource.TrimMaterial;

        //Details = new List<MaterialType>();
        //Details.AddRange(Details);
        Instantiation = null;

        Debug.Log("Copy Hand Complete!");
    }
    public override bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (!base.UpdateCharacterRender(character))
            return false;

        character.GameState.SceneMan.InstantiateHandEquip(this, character.Render, putOn);

        return true;
    }

    /*public override bool EquipToCharacter(Character character, int slotIndex = -1)
    {
        return base.EquipToCharacter(character, slotIndex);
    }*/

}
