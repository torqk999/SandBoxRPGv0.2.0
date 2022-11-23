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

    public virtual DraggableButton GenerateMyButton(ButtonOptions options)
    {
        options.ButtonType = ButtonType.DEFAULT; // test point, shouldn't proc
        GameObject buttonObject = RootLogic.GameState.UIman.GenerateButtonObject(options);
        DraggableButton myButton = buttonObject.AddComponent<DraggableButton>();
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
}
