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
        RootLogic.Options = options;

        Name = source.Name;
        Flavour = source.Flavour;
        sprite = source.sprite;
    }

    public virtual RootButton GenerateMyButton(ButtonOptions options)
    {
        options.ButtonType = ButtonType.DEFAULT; // test point, shouldn't proc
        GameObject buttonObject = RootLogic.GameState.UIman.GenerateButtonObject(options);
        RootButton myButton = buttonObject.AddComponent<RootButton>();
        myButton.Init(options);
        return myButton;
    }
    public virtual RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == string.Empty ? "RootScriptObject" : options.ClassID;
        RootScriptObject newRootObject = (RootScriptObject)CreateInstance(options.ClassID);
        newRootObject.RootLogic.GameState = RootLogic.GameState;
        newRootObject.Copy(this, options);
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
        if (RootLogic.Page == null ||
            RootLogic.Page.Occupants.List == null ||
            RootLogic.Options.Index < 0 ||
            RootLogic.Options.Index >= RootLogic.Page.Occupants.List.Count)
            return false;

        RootLogic.Page.Occupants.List[RootLogic.Options.Index] = null;
        return true;
        // Un parent? Should be re-parenting anyway...
        //return base.Vacate();
    }
    public bool Relocate()
    {
        if (RootLogic.Page == null ||
            RootLogic.Page.Occupants == null ||
            RootLogic.Page.Occupants.List == null)
            return false;

        int emptyIndex = RootLogic.Page.Occupants.ReturnEmptyIndex();
        if (emptyIndex == -1)
        {
            return Drop();
        }

        RootLogic.Page.Occupants.List[RootLogic.Options.Index] = null;
        RootLogic.Options.Index = emptyIndex;
        RootLogic.Page.Occupants.List[RootLogic.Options.Index] = this;
        return true;
    }
    public bool CheckCanOccupy(RootScriptObject place)
    {
        if (place == null)
            return true;

        if (place == this) //.Panel.VirtualParent.Occupants.List[ButtonTarget.SlotIndex] == this)
            return false; // already there;

        if (!place.Vacate())
            return false;

        return true;
    }
    public virtual bool Occupy(Page place, int index)
    {
        if (!CheckCanOccupy(place.Occupants.List[index]))
            return false;

        //Panel = place.Panel.VirtualParent.Occupants;
        RootLogic.Options.Index = index;
        RootLogic.Page.Occupants.List[RootLogic.Options.Index] = this;
        RootLogic.Page.PlaceHolders.List[RootLogic.Options.Index].Assign(this);

        return true;
    }
    #endregion
}
