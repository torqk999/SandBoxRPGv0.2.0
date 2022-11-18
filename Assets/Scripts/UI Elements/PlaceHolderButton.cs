using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(PlaceHolderButton))]
public class PlaceHolderButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        PlaceHolderButton button = (PlaceHolderButton)target;
    }
}

public class PlaceHolderButton : SelectableButton
{
    [Header("PlaceHolder")]
    public DraggableButton Occupant;

    public override bool Vacate()
    {
        if (Occupant == null)
            return true;
        return Occupant.Vacate();
    }

    public void ResetImage()
    {
        if (Root == null || Root.Sprite == null)
            MyImage.sprite = UIMan.PlaceHolderSprite;
        else
            MyImage.sprite = Root.Sprite;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ResetImage();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
