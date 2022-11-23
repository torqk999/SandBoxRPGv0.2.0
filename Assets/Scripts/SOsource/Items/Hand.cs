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
        options.ClassID = options.ClassID == "" ? "Hand" : options.ClassID;
        Hand newRoot = (Hand)base.GenerateRootObject(options);
        newRoot.Copy(this, options);
        return newRoot;
    }

    public override void Copy(RootScriptObject source, RootOptions options)
    {
        base.Copy(source, options);

        if (!(source is Hand))
            return;

        Hand handSource = (Hand)source;

        HandPosition = handSource.HandPosition;
        BaseMaterial = handSource.BaseMaterial;
        HandleMaterial = handSource.HandleMaterial;
        TrimMaterial = handSource.TrimMaterial;

        //Details = new List<MaterialType>();
        //Details.AddRange(Details);
        Instantiation = null;
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
