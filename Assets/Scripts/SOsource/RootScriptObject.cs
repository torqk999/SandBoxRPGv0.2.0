using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScriptObject : ScriptableObject
{
    [Header("Root Properties")]
    public string Name;
    public string Flavour;
    public Sprite sprite;

    [Header("Root Logic - NO TOUCHY!")]
    public RootLogic RootLogic;

    public virtual void TextBuilder(ref StringBuilder title, ref StringBuilder stats, ref StringBuilder flavour)
    {
        title.Append(Name);
        flavour.Append(Flavour);
    }
    public virtual void Clone(RootOptions options)
    {
        //Debug.Log("yo");
        RootLogic.Copy(options); //Debug.Log("yo");

        Name = options.Source.Name; //Debug.Log("yo");
        Flavour = options.Source.Flavour; //Debug.Log("yo");
        sprite = options.Source.sprite; //Debug.Log("yo");

        Debug.Log("Copy Root Complete!");
    }
    public virtual RootScriptObject GenerateRootObject(RootOptions options)
    {
        Debug.Log($"Generating...: {options.Source.GetType()}");
        RootScriptObject newRootObject = (RootScriptObject)CreateInstance(options.Source.GetType());
        newRootObject.RootLogic.Options.GameState = RootLogic.Options.GameState;
        newRootObject.Clone(options);
        Debug.Log($"Generated! : {options.Source.GetType()}");
        return newRootObject;
    }
    public virtual void InitializeRoot(GameState state)
    {
        Debug.Log("Initializing Root...");
        RootLogic.Options.GameState = state;
    }

    #region BUTON ACTIONS
    public virtual bool Drop()
    {
        return Vacate();
    }
    public virtual bool Vacate()
    {
        if (RootLogic.Options.HomePanel == null ||
            RootLogic.Options.Index < 0 ||
            RootLogic.Options.Index >= RootLogic.Options.HomePanel.Count)
            return false;

        RootLogic.Options.HomePanel[RootLogic.Options.Index] = null;
        return true;
        // Un parent? Should be re-parenting anyway...
        //return base.Vacate();
    }
    public bool Relocate()
    {
        if (RootLogic.Options.HomePanel == null)
            return false;

        int emptyIndex = -1;// RootLogic.Options.GameState.UIman..ReturnEmptyIndex();
        for (int i = 0; i < RootLogic.Options.HomePanel.Count; i++)
            if (RootLogic.Options.HomePanel[i] == null)
            {
                emptyIndex = i;
                break;
            }

        if (emptyIndex == -1)
        {
            return Drop();
        }

        RootLogic.Options.HomePanel[RootLogic.Options.Index] = null;
        RootLogic.Options.Index = emptyIndex;
        RootLogic.Options.HomePanel[RootLogic.Options.Index] = this;
        return true;
    }
    public bool CheckCanOccupy(Page page, int index)
    {
        switch(page.PlaceType)
        {
            case PlaceHolderType.CHARACTER:
                if (!(this is CharacterSheet))
                    return false;
                break;

            case PlaceHolderType.INVENTORY:
                if (!(this is ItemObject))
                    return false;
                break;

            case PlaceHolderType.EQUIP:
                if (!(this is Equipment))
                    return false;
                break;

            case PlaceHolderType.SKILL:
                if (!(this is CharacterAbility))
                    return false;
                break;

            case PlaceHolderType.EFFECT:
                if (!(this is BaseEffect))
                    return false;
                break;

            case PlaceHolderType.HOT_BAR:
                if (!(this is ItemObject) &&
                    !(this is CharacterAbility))
                    return false;
                break;
        }

        if (page.OccupantRoots[index] == this)
            return false; // already there;

        if (page.OccupantRoots[index] == null)
            return true; // Empty

        if (!page.OccupantRoots[index].Vacate())
            return false; // Failed to vacate

        return true; // Successfully vacated
    }
    public virtual bool Occupy(Page page, int index)
    {
        if (!CheckCanOccupy(page, index))
            return false;

        //Panel = place.Panel.VirtualParent.Occupants;
        RootLogic.Options.Index = index;
        page.OccupantRoots[index] = this;
        page.Buttons.List[index].Assign(this);

        return true;
    }
    #endregion
}
