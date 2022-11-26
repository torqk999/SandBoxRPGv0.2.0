using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(RootUI))]
public class RootButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        RootUI button = (RootUI)target;
    }
}




public class RootUI : SelectableUI
{
    public RootScriptObject Root;
    //public Image MyImage;
    //public PlaceHolderType PlaceType;
    public Sprite EmptyPlaceHolder;

    public void UpdateSprite()
    {
        if (Root == null)
            MyImage.sprite = EmptyPlaceHolder;
        else
            MyImage.sprite = Root.sprite;
    }

    public override void Init(UI_Options options)
    {
        Debug.Log("Init root");
        base.Init(options);
        if (options.ResetImage)
            EmptyPlaceHolder = UIMan.EmptyButtonSprite;
        else
            EmptyPlaceHolder = MyImage.sprite;
        UpdateSprite();

        Debug.Log("Root init done!");
    }

    public override void Assign(RootScriptObject root)
    {
        if (root != null)
        Debug.Log($"Assigning: {root.Name}");
        base.Assign(root);
        Root = root;
        if (Root != null)
            Root.TextBuilder(ref Title, ref Stats, ref Flavour);
        
        UpdateSprite();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //MyImage = GetComponent<Image>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
