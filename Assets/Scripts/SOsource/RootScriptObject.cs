using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScriptObject : ScriptableObject
{
    [Header("Root Properties")]
    public GameState GameState;
    //public int ID;
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
        GameObject buttonObject = GameState.UIman.GenerateButtonObject(options);
        DraggableButton myButton = buttonObject.GetComponent<DraggableButton>();
        myButton.Init(options, this);
        return myButton;
    }
    public virtual RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == string.Empty ? "RootScriptObject" : options.ClassID;

        //Debug.Log($"Generating Root Object: {options.ClassID}");

        RootScriptObject newRootObject = (RootScriptObject)CreateInstance(options.ClassID);
        newRootObject.GameState = GameState;
        newRootObject.Copy(this, options);
        return newRootObject;
    }

    public virtual void InitializeRoot(GameState state)
    {
        GameState = state;
    }
}
