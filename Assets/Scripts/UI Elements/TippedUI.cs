using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

using UnityEditor;

[CustomEditor(typeof(TippedUI))]
public class TippedButtonButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Show default inspector property editor
        DrawDefaultInspector();
        TippedUI button = (TippedUI)target;
    }
}

public class TippedUI : ExtendedUI
{
    [Header("TippedButton")]
    //public StringBuilder Title;
    //public StringBuilder Stats;
    //public StringBuilder Flavour;
    public StringBuilder[] Strings;

    public override void OnPointerEnter(PointerEventData eventData)
    {
        UIMan.ToolTip.ToggleTip(true, Strings);
        //Stats.Clear();
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        UIMan.ToolTip.ToggleTip(false);
        base.OnPointerExit(eventData);
    }
    public override void Assign(RootScriptObject root)
    {
        base.Assign(root);

        for (int i = 0; i < 3; i++)
            Strings[i].Clear();
    }
    public override void Init(UI_Options options)
    {
        Debug.Log("Init tip");
        base.Init(options);
        Strings = new StringBuilder[3];
        for (int i = 0; i < 3; i++)
            Strings[i] = new StringBuilder();
        //Title = new StringBuilder();
        //Stats = new StringBuilder();
        //Flavour = new StringBuilder();
        Debug.Log("Tipped done");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start(); 
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
