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
    public virtual void Clone(RootScriptObject source, RootOptions options)
    {
        ID = options.ID;

        Name = source.Name;
        Flavour = source.Flavour;
        Sprite = source.Sprite;
    }

    public virtual void GenerateMyButton()
    {

    }
    public virtual RootScriptObject GenerateRootObject(RootOptions options)
    {
        options.ClassID = options.ClassID == string.Empty ? "RootScriptObject" : options.ClassID;

        Debug.Log($"Generating Root Object: {options.ClassID}");

        RootScriptObject newRootObject = (RootScriptObject)CreateInstance(options.ClassID);
        newRootObject.GameState = GameState;
        newRootObject.Clone(this, options);
        return newRootObject;
    }

    public virtual void InitializeRoot(GameState state)
    {
        GameState = state;
    }
}
