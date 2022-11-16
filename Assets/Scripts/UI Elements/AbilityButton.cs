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
public class AbilityButton : SelectableButton
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }
}
