using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
//using System.Runtime.InteropServices;

using UnityEditor;

[CustomEditor(typeof(AbilityButton))]
public class AbilityButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        AbilityButton button = (AbilityButton)target;
    }
}
public class AbilityButton : DraggableButton
{

    public override void Init(ButtonOptions options, RootScriptObject root = null)
    {
        base.Init(options, root);
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        
    }
}
