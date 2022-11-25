using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScriptObject : ScriptableObject
{
    [Header("Root Properties")]
    //public GameState GameState;
    public string Name;
    public string Flavour;
    public Sprite sprite;

    [Header("Root Logic - NO TOUCHY!")]
    public RootLogic RootLogic;
    
    public virtual void Copy(RootScriptObject source, RootOptions options)
    {
        Debug.Log("yo");
        RootLogic.Options = options; Debug.Log("yo");
        Name = source.Name; Debug.Log("yo");
        Flavour = source.Flavour; Debug.Log("yo");
        sprite = source.sprite; Debug.Log("yo");

        Debug.Log("Copy Root Complete!");
    }

    /*public virtual RootButton GenerateMyButton(ButtonOptions options)
    {
        options.PlaceType = PlaceHolderType.NONE; // test point, shouldn't proc
        GameObject buttonObject = RootLogic.GameState.UIman.GenerateButtonObject(options);
        RootButton myButton = buttonObject.AddComponent<RootButton>();
        myButton.Init(options);
        return myButton;
    }*/
    public virtual RootScriptObject GenerateRootObject(RootOptions options)
    {
        Debug.Log($"Generating...: {options.Root.GetType()}");
        RootScriptObject newRootObject = (RootScriptObject)CreateInstance(options.Root.GetType());
        newRootObject.RootLogic.GameState = RootLogic.GameState;
        Debug.Log("Good so far");
        newRootObject.Copy(this, options);
        Debug.Log($"Generated! : {options.Root.GetType()}");
        return newRootObject;
    }

    public virtual void InitializeRoot(GameState state)
    {
        RootLogic.GameState = state;
    }

    #region BUTON ACTIONS
    public virtual bool Drop()
    {
        return Vacate();
    }
    public virtual bool Vacate()
    {
        if (RootLogic.Options.Page == null ||
            RootLogic.Options.Page.OccupantRoots.List == null ||
            RootLogic.Options.Index < 0 ||
            RootLogic.Options.Index >= RootLogic.Options.Page.OccupantRoots.List.Count)
            return false;

        RootLogic.Options.Page.OccupantRoots.List[RootLogic.Options.Index] = null;
        return true;
        // Un parent? Should be re-parenting anyway...
        //return base.Vacate();
    }
    public bool Relocate()
    {
        if (RootLogic.Options.Page == null ||
            RootLogic.Options.Page.OccupantRoots == null ||
            RootLogic.Options.Page.OccupantRoots.List == null)
            return false;

        int emptyIndex = RootLogic.Options.Page.OccupantRoots.ReturnEmptyIndex();
        if (emptyIndex == -1)
        {
            return Drop();
        }

        RootLogic.Options.Page.OccupantRoots.List[RootLogic.Options.Index] = null;
        RootLogic.Options.Index = emptyIndex;
        RootLogic.Options.Page.OccupantRoots.List[RootLogic.Options.Index] = this;
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

        if (page.OccupantRoots.List[index] == this)
            return false; // already there;

        if (page.OccupantRoots.List[index] == null)
            return true; // Empty

        if (!page.OccupantRoots.List[index])
            return false; // Failed to vacate

        return true; // Successfully vacated
    }
    public virtual bool Occupy(Page place, int index)
    {
        if (!CheckCanOccupy(place, index))
            return false;

        //Panel = place.Panel.VirtualParent.Occupants;
        RootLogic.Options.Index = index;
        RootLogic.Options.Page.OccupantRoots.List[RootLogic.Options.Index] = this;
        RootLogic.Options.Page.Buttons.List[RootLogic.Options.Index].Assign(this);

        return true;
    }
    #endregion
}
