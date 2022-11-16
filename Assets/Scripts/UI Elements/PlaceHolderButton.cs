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
    public Image PlaceHolder;

    // Start is called before the first frame update
    protected override void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
