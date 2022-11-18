using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootScriptObject : ScriptableObject
{
    [Header("Root Properties")]
    public GameState GameState;
    public int ID;
    public string Name;
    public string Flavour;
    public Sprite Sprite;

    [Header("Root Logic - NO TOUCHY!")]
    public RootLogic RootLogic;
    public DraggableButton Button;
    public virtual void Copy(RootScriptObject source, RootOptions options)
    {
        ID = options.ID;

        Name = source.Name;
        Flavour = source.Flavour;
        Sprite = source.Sprite;
    }

    public virtual DraggableButton GenerateMyButton(ButtonOptions options)
    {
        options.Type = ButtonType.DEFAULT; // test point, shouldn't proc
        GameObject buttonObject = GameState.UIman.GenerateButtonObject(options);
        return buttonObject.AddComponent<DraggableButton>();
    }
    public virtual RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == string.Empty ? "RootScriptObject" : options.ClassID;

        Debug.Log($"Generating Root Object: {options.ClassID}");

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
