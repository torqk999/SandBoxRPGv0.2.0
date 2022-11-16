using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(SelectableButton))]
public class SelectableButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        SelectableButton button = (SelectableButton)target;
    }
}

public class SelectableButton : TippedButton
{
    [Header("SelectableButton")]
    public bool Selected;
    public int Index;

    [Header("Debugging")]
    
    public Color DefaultColor;
    public Color SelectionColor;

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        UIMan.CharacterPageSelection(this);
        Selected = true;
        MyImage.color = SelectionColor;
    }

    public void UnSelect()
    {
        Selected = false;
        MyImage.color = DefaultColor;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        DefaultColor = MyImage.color;
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
