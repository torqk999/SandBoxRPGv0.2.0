using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : Equipment
{
    [Header("Hand Properties")]
    public HandPosition HandPosition;
    public MaterialType BaseMaterial;
    public MaterialType HandleMaterial;
    public MaterialType TrimMaterial;
    //public List<MaterialType> Details;
    public GameObject Instantiation;   // Used by equipping only

    public override ItemObject GenerateItem(int equipId = -1, bool inject = false)
    {
        Hand hand = (Hand)CreateInstance("Hand");
        hand.CloneItem(this, equipId, inject);
        return hand;
    }

    public override void CloneItem(ItemObject source, int equipId = -1, bool inject = false, int quantity = 1)
    {
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

        base.CloneItem(source);
    }
    public override bool UpdateCharacterRender(Character character, bool putOn = true)
    {
        if (!base.UpdateCharacterRender(character))
            return false;

        character.GameState.SceneMan.InstantiateHandEquip(this, character.Render, putOn);

        return true;
    }

    /*public override bool EquipCharacter(Character character, int inventorySlot, int destinationIndex = 0)
    {
        if (!base.EquipCharacter(character, inventorySlot))
            return false;

        return true;
    }*/

}
