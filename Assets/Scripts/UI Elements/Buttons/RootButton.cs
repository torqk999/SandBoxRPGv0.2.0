using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(RootButton))]
public class RootButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        RootButton button = (RootButton)target;
    }
}




public class RootButton : SelectableButton
{
    public RootScriptObject Root;
    //public Image MyImage;
    //public PlaceHolderType PlaceType;
    public Sprite EmptyPlaceHolder;

    public void TargetRoot(RootScriptObject root)
    {
        Root = root;
        if (Root == null)
            MyImage.sprite = EmptyPlaceHolder;
        else
            MyImage.sprite = Root.sprite;
    }

    public override void Init(ButtonOptions options)
    {
        base.Init(options);
        if (options.ResetImage)
            EmptyPlaceHolder = UIMan.EmptyButtonSprite;
        else
            EmptyPlaceHolder = MyImage.sprite;
        TargetRoot(options.Root);
    }

    public override void Assign(RootScriptObject root)
    {
        base.Assign(root);
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
